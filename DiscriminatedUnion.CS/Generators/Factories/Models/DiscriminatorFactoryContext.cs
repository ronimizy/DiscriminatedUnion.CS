using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Factories.Models;

public record struct DiscriminatorFactoryContext(
    ClassDeclarationSyntax UnionTypeSyntax,
    INamedTypeSymbol UnionTypeSymbol,
    INamedTypeSymbol DiscriminatorInterfaceSymbol,
    INamedTypeSymbol NamedDiscriminatorInterfaceSymbol,
    SemanticModel SemanticModel,
    CancellationToken CancellationToken);