using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct DiscriminatorTypeBuildingContext(
    TypeDeclarationSyntax TypeDeclaration,
    UnionType UnionType,
    Discriminator Discriminator,
    string FieldName);