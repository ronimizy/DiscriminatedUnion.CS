using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct UnionBuildingContext(
    TypeDeclarationSyntax TypeDeclarationSyntax,
    UnionType UnionType,
    IReadOnlyCollection<Discriminator> Discriminators);