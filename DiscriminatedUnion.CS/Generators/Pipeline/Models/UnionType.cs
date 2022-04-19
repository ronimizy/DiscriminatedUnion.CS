using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct UnionType(INamedTypeSymbol Symbol, string Name);