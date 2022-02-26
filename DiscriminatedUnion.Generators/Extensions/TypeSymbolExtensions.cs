using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Extensions
{
    public static class TypeSymbolExtensions
    {
        public static bool DerivesOrConstructedFrom(this INamedTypeSymbol namedTypeSymbol, INamedTypeSymbol baseType)
        {
            if (namedTypeSymbol.EqualsDefault(baseType))
                return true;

            if (namedTypeSymbol.ConstructedFrom.EqualsDefault(baseType))
                return true;

            if (namedTypeSymbol.BaseType is not null && namedTypeSymbol.BaseType.DerivesOrConstructedFrom(baseType))
                return true;

            if (!namedTypeSymbol.ConstructedFrom.EqualsDefault(namedTypeSymbol) &&
                namedTypeSymbol.ConstructedFrom.DerivesOrConstructedFrom(baseType))
                return true;

            return false;
        }
    }
}