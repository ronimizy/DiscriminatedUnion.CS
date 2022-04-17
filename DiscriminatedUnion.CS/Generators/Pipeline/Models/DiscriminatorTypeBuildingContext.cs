using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct DiscriminatorTypeBuildingContext(
    TypeDeclarationSyntax TypeDeclaration,
    INamedTypeSymbol UnionTypeSymbol,
    INamedTypeSymbol WrappedTypeSymbol,
    string WrappedTypeName,
    string DiscriminatorTypeName,
    string FieldName);