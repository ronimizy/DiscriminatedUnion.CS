using DiscriminatedUnion.CS.Analyzers;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators;

[Generator]
public class DiscriminatedUnionSourceGenerator : ISourceGenerator
{
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

        if (unionAttribute is null || discriminatorInterface is null)
            return;

        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        foreach (var syntax in receiver.Nodes)
        {
            ProcessSyntaxNode(context, syntax, unionAttribute, discriminatorInterface);
        }
    }

    private void ProcessSyntaxNode(
        GeneratorExecutionContext generatorContext,
        SyntaxNode syntax,
        INamedTypeSymbol unionAttribute,
        INamedTypeSymbol discriminatorInterface)
    {
        var model = generatorContext.Compilation.GetSemanticModel(syntax.SyntaxTree);

        if (model.GetDeclaredSymbol(syntax) is not INamedTypeSymbol unionTypeSymbol)
            return;

        if (!unionTypeSymbol.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
            return;

        if (!DiscriminatedUnionBaseRequirementsAnalyzer.IsTypeCompliant(unionTypeSymbol))
            return;

        Discriminator[] discriminators = unionTypeSymbol.Interfaces
            .Where(i => i.DerivesOrConstructedFrom(discriminatorInterface))
            .Select(i => ExtractWrappedType(i, discriminatorInterface))
            .Select(t => new Discriminator(t, t.ToNameSyntax(fullyQualified: true), IdentifierName(t.Name)))
            .ToArray();

        var unionType = new UnionType(unionTypeSymbol, unionTypeSymbol.ToNameSyntax());

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
        const string fieldName = "_value";
        TypeDeclarationSyntax typeSyntax = ClassDeclaration(discriminator.Name.Identifier);

        var wrappedContext = new DiscriminatorTypeBuildingContext(
            typeSyntax,
            unionType,
            discriminator,
            fieldName);

        return _discriminatorBuilder.BuildDiscriminatorTypeSyntax(wrappedContext);
    }

    private static INamedTypeSymbol ExtractWrappedType(INamedTypeSymbol i, INamedTypeSymbol discriminatorInterface)
    {
        while (!i.ConstructedFrom.EqualsDefault(discriminatorInterface))
        {
            i = i.ConstructedFrom;
        }

        return i.TypeArguments.OfType<INamedTypeSymbol>().Single();
    }
}