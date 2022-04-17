using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct UnionBuildingContext(
    TypeDeclarationSyntax TypeDeclarationSyntax,
    INamedTypeSymbol UnionTypeSymbol,
    string UnionName);