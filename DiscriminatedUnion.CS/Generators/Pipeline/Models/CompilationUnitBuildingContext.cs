using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct CompilationUnitBuildingContext(
    CompilationUnitSyntax Syntax,
    UnionType UnionType,
    IReadOnlyCollection<Discriminator> Discriminators);