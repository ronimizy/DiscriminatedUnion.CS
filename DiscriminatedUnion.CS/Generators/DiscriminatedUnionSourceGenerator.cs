using System.Collections.Immutable;
using DiscriminatedUnion.CS.Analyzers;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators;

[Generator]
public class DiscriminatedUnionSourceGenerator : ISourceGenerator
{
    private const string FieldName = "_value";
    private static readonly IdentifierNameSyntax FieldNameIdentifier = IdentifierName(FieldName);

    private readonly ICompilationUnitBuilder _compilationUnitBuilder;
    private readonly IDiscriminatorBuilder _discriminatorBuilder;
    private readonly IUnionBuilder _unionBuilder;

    public DiscriminatedUnionSourceGenerator()
    {
        var collection = new ServiceCollection();
        collection.AddBuilders();

        var provider = collection.BuildServiceProvider();

        _compilationUnitBuilder = provider.GetServices<ICompilationUnitBuilder>().Aggregate((a, b) => a.AddNext(b));
        _discriminatorBuilder = provider.GetServices<IDiscriminatorBuilder>().Aggregate((a, b) => a.AddNext(b));
        _unionBuilder = provider.GetServices<IUnionBuilder>().Aggregate((a, b) => a.AddNext(b));
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var unionAttribute = context.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatedUnionAttributeFullyQualifiedName);

        var discriminatorInterface = context.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatorInterfaceFullyQualifiedName);

        var namedDiscriminatorInterface = context.Compilation
            .GetTypeByMetadataName(Definer.NamedDiscriminatorInterfaceFullyQualifiedName);

        if (unionAttribute is null || discriminatorInterface is null || namedDiscriminatorInterface is null)
            return;

        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        foreach (var syntax in receiver.Nodes)
        {
#if RELEASE
            try
            {
#endif
            GenerateUnionType(context, syntax, unionAttribute, discriminatorInterface, namedDiscriminatorInterface);
#if RELEASE
            }
            catch (Exception _) { }
#endif
        }
    }

    private void GenerateUnionType(
        GeneratorExecutionContext generatorContext,
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol unionAttribute,
        INamedTypeSymbol discriminatorInterface,
        INamedTypeSymbol namedDiscriminatorInterface)
    {
        var model = generatorContext.Compilation.GetSemanticModel(syntax.SyntaxTree);

        if (model.GetDeclaredSymbol(syntax) is not INamedTypeSymbol unionTypeSymbol)
            return;

        if (!unionTypeSymbol.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
            return;

        if (!DiscriminatedUnionBaseRequirementsAnalyzer.IsTypeCompliant(unionTypeSymbol))
            return;

        var unionType = new UnionType(unionTypeSymbol, unionTypeSymbol.ToNameSyntax());

        if (syntax.BaseList is null)
            return;

        ILookup<NamedDiscriminatorType, BaseTypeSyntax> namedDiscriminatorInterfaces = syntax.BaseList.Types
            .Where(s => s.DerivesOrConstructedFrom(model, namedDiscriminatorInterface))
            .ToLookup(s => NamedDiscriminatorAnalyzer.AnalyzeNamedDiscriminator(s, model));

        if (namedDiscriminatorInterfaces[NamedDiscriminatorType.Invalid].Any())
            return;

        var existingNamedDiscriminators = namedDiscriminatorInterfaces[NamedDiscriminatorType.Exising]
            .Select(s => model.GetTypeInfo(s.Type).Type)
            .OfType<INamedTypeSymbol>()
            .Select(i => ExtractWrappedTypes(i, namedDiscriminatorInterface))
            .Select(t => new Discriminator(t[0], t[0].ToNameSyntax(true), IdentifierName(t[1].Name)));

        var nonGeneratedNamedDiscriminators = namedDiscriminatorInterfaces[NamedDiscriminatorType.NonGenerated]
            .Select(s => model.GetTypeInfo(s.Type).Type)
            .OfType<INamedTypeSymbol>()
            .Select(i => ExtractWrappedTypes(i, namedDiscriminatorInterface))
            .Select(t => new NonGeneratedDiscriminator(t[0], t[0].ToNameSyntax(true), IdentifierName(t[1].Name)));

        Discriminator[] discriminators = unionTypeSymbol.Interfaces
            .Where(i => i.DerivesOrConstructedFrom(discriminatorInterface))
            .Select(i => ExtractWrappedType(i, discriminatorInterface))
            .Select(t => new Discriminator(t, t.ToNameSyntax(fullyQualified: true), IdentifierName(t.Name)))
            .Concat(existingNamedDiscriminators)
            .Concat(nonGeneratedNamedDiscriminators)
            .ToArray();

        SimpleNameSyntax[] wrappedTypes = discriminators.Select(d => d.Name).ToArray();

        if (ConflictingNameAnalyzer.GetAmbiguouslyNamedTypes(syntax, wrappedTypes).Any())
            return;

        TypeDeclarationSyntax unionTypeSyntax = ClassDeclaration(unionType.Name.Identifier);
        var unionBuildingContext = new UnionBuildingContext(unionTypeSyntax, unionType, discriminators);
        unionTypeSyntax = _unionBuilder.BuildUnionTypeSyntax(unionBuildingContext);

        foreach (var discriminator in discriminators)
        {
            var discriminatorTypeSyntax = GenerateDiscriminator(unionType, discriminator);
            unionTypeSyntax = unionTypeSyntax.AddMembers(discriminatorTypeSyntax);
        }

        var namespaceSyntax = NamespaceDeclaration(unionType.Symbol.ContainingNamespace.ToNameSyntax(true))
            .AddMembers(unionTypeSyntax);

        var context = new CompilationUnitBuildingContext(CompilationUnit(), unionType, discriminators);
        var compilationUnit = _compilationUnitBuilder
            .BuildCompilationUnitSyntax(context)
            .AddMembers(namespaceSyntax);

        var hintName = $"{unionType.Symbol.GetFullyQualifiedName()}{Definer.FilenameSuffix}";
        var source = compilationUnit.NormalizeWhitespace().ToFullString();

        generatorContext.AddSource(hintName, source);
    }

    private TypeDeclarationSyntax GenerateDiscriminator(UnionType unionType, Discriminator discriminator)
    {
        TypeDeclarationSyntax typeSyntax = ClassDeclaration(discriminator.Name.Identifier);

        var wrappedContext = new DiscriminatorTypeBuildingContext(
            typeSyntax,
            unionType,
            discriminator,
            FieldNameIdentifier);

        return _discriminatorBuilder.BuildDiscriminatorTypeSyntax(wrappedContext);
    }

    private static ITypeSymbol ExtractWrappedType(INamedTypeSymbol i, INamedTypeSymbol discriminatorInterface)
        => ExtractClosestDerivation(i, discriminatorInterface).TypeArguments.OfType<ITypeSymbol>().Single();

    private static ImmutableArray<ITypeSymbol> ExtractWrappedTypes(
        INamedTypeSymbol i,
        INamedTypeSymbol discriminatorInterface)
        => ExtractClosestDerivation(i, discriminatorInterface).TypeArguments;

    private static INamedTypeSymbol ExtractClosestDerivation(INamedTypeSymbol i, INamedTypeSymbol interfaceType)
    {
        while (!i.ConstructedFrom.EqualsDefault(interfaceType))
        {
            i = i.ConstructedFrom;
        }

        return i;
    }
}