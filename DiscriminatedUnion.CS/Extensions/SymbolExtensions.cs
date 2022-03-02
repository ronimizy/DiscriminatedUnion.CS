using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Extensions;

public static class SymbolExtensions
{
    public static bool EqualsDefault(this ISymbol left, ISymbol? right)
        => left.Equals(right, SymbolEqualityComparer.Default);
}