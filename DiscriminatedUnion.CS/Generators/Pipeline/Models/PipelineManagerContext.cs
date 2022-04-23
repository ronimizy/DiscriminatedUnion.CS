using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct PipelineManagerContext(
    INamedTypeSymbol UnionAttributeSymbol,
    INamedTypeSymbol DiscriminatorInterfaceSymbol,
    INamedTypeSymbol NamedDiscriminatorInterfaceSymbol,
    SemanticModel SemanticModel,
    ClassDeclarationSyntax UnionTypeSyntax,
    INamedTypeSymbol UnionTypeSymbol,
    Action<string, string> SubmitSourceAction,
    CancellationToken CancellationToken);