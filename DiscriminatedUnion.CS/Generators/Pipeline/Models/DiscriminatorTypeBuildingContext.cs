using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct DiscriminatorTypeBuildingContext(
    TypeDeclarationSyntax TypeDeclaration,
    INamedTypeSymbol UnionTypeSymbol,
    Discriminator Discriminator,
    string FieldName);