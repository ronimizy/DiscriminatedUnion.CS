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

        if (model.GetDeclaredSymbol(syntax) is not INamedTypeSymbol unionType)
            return;

        if (!unionType.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
            return;

        INamedTypeSymbol[] wrappedTypeSymbols = unionType.Interfaces
            .Where(i => i.DerivesOrConstructedFrom(discriminatorInterface))
            .Select(i => ExtractWrappedType(i, discriminatorInterface))
            .ToArray();

        var unionTypeName = unionType.Name;

        TypeDeclarationSyntax unionTypeSyntax = ClassDeclaration(unionTypeName);
        var unionBuildingContext = new UnionBuildingContext(unionTypeSyntax, unionType, unionTypeName);
        unionTypeSyntax = _unionBuilder.BuildUnionTypeSyntax(unionBuildingContext);

        foreach (var wrappedTypeSymbol in wrappedTypeSymbols)
        {
            var discriminatorTypeSyntax = GenerateDiscriminator(unionType, wrappedTypeSymbol);
            unionTypeSyntax = unionTypeSyntax.AddMembers(discriminatorTypeSyntax);
        }

        var namespaceSyntax = NamespaceDeclaration(IdentifierName(unionType.ContainingNamespace.GetFullyQualifiedName()))
            .AddMembers(unionTypeSyntax);

        var context = new CompilationUnitBuildingContext(CompilationUnit(), unionType, wrappedTypeSymbols);
        var compilationUnit = _compilationUnitBuilder
            .BuildCompilationUnitSyntax(context)
            .AddMembers(namespaceSyntax);

        var hintName = $"{unionType.GetFullyQualifiedName(true)}{Definer.FilenameSuffix}";
        var source = compilationUnit.NormalizeWhitespace().ToFullString();

        generatorContext.AddSource(hintName, source);
    }

    private TypeDeclarationSyntax GenerateDiscriminator(
        INamedTypeSymbol unionTypeSymbol,
        INamedTypeSymbol wrappedTypeSymbol)
    {
        const string fieldName = "_value";
        var wrappedTypeName = wrappedTypeSymbol.GetFullyQualifiedName();
        var discriminatorTypeName = wrappedTypeSymbol.Name;

        var discriminator = new Discriminator(wrappedTypeSymbol, wrappedTypeName, discriminatorTypeName);

        TypeDeclarationSyntax typeSyntax = ClassDeclaration(discriminator.Name);

        var wrappedContext = new DiscriminatorTypeBuildingContext(
            typeSyntax,
            unionTypeSymbol,
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