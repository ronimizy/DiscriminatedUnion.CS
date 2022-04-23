using DiscriminatedUnion.CS.Analyzers;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Factories;
using DiscriminatedUnion.CS.Generators.Factories.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline;

public class PipelineManager
{
    private readonly ICompilationUnitBuilder _compilationUnitBuilder;
    private readonly IUnionBuilder _unionBuilder;
    private readonly DiscriminatorFactory _discriminatorFactory;

    public PipelineManager(
        IEnumerable<ICompilationUnitBuilder> compilationUnitBuilders,
        IEnumerable<IUnionBuilder> unionBuilders,
        DiscriminatorFactory discriminatorFactory)
    {
        _discriminatorFactory = discriminatorFactory;
        _compilationUnitBuilder = compilationUnitBuilders.Aggregate();
        _unionBuilder = unionBuilders.Aggregate();
    }

    public void GenerateDiscriminatedUnion(PipelineManagerContext context)
    {
        var (_,
            discriminatorInterfaceSymbol,
            namedDiscriminatorInterfaceSymbol,
            semanticModel,
            unionTypeSyntax,
            unionTypeSymbol,
            submitSourceAction,
            cancellationToken) = context;

        cancellationToken.ThrowIfCancellationRequested();

        var unionType = new UnionType(unionTypeSymbol, unionTypeSymbol.ToNameSyntax());

        var discriminatorFactoryContext = new DiscriminatorFactoryContext(
            unionTypeSyntax,
            unionTypeSymbol,
            discriminatorInterfaceSymbol,
            namedDiscriminatorInterfaceSymbol,
            semanticModel,
            cancellationToken);

        (var hasInvalidDiscriminators, IEnumerable<Discriminator> discriminatorEnumerable) = _discriminatorFactory
            .BuildDiscriminators(discriminatorFactoryContext);

        if (hasInvalidDiscriminators)
            return;

        Discriminator[] discriminators = discriminatorEnumerable.ToArray();
        SimpleNameSyntax[] wrappedTypes = discriminators.Select(d => d.Name).ToArray();

        if (ConflictingNameAnalyzer.GetAmbiguouslyNamedTypes(unionTypeSyntax, wrappedTypes).Any())
            return;

        TypeDeclarationSyntax discriminatedUnionTypeSyntax = ClassDeclaration(unionType.Name.Identifier);
        var unionBuildingContext = new UnionBuildingContext(discriminatedUnionTypeSyntax, unionType, discriminators);
        discriminatedUnionTypeSyntax = _unionBuilder.BuildUnionTypeSyntax(unionBuildingContext);

        var namespaceSyntax = NamespaceDeclaration(unionType.Symbol.ContainingNamespace.ToNameSyntax(true))
            .AddMembers(discriminatedUnionTypeSyntax);

        var compilationUnitBuildingContext = new CompilationUnitBuildingContext(
            CompilationUnit(), unionType, discriminators);

        var compilationUnit = _compilationUnitBuilder
            .BuildCompilationUnitSyntax(compilationUnitBuildingContext)
            .AddMembers(namespaceSyntax);

        var hintName = $"{unionType.Symbol.GetFullyQualifiedName()}{Definer.FilenameSuffix}";
        var source = compilationUnit.NormalizeWhitespace().ToFullString();

        submitSourceAction.Invoke(hintName, source);
    }
}