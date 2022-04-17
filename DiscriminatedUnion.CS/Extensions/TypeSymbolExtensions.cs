using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

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

    public static TypeSyntax ToTypeSyntax(this INamedTypeSymbol symbol)
    {
        IdentifierNameSyntax[] typeParameters = symbol.TypeParameters
            .Select(t => IdentifierName(t.Name))
            .ToArray();

        return typeParameters.Length is 0
            ? IdentifierName(symbol.Name)
            : GenericName(Identifier(symbol.Name), TypeArgumentList(SeparatedList<TypeSyntax>(typeParameters)));
    }
}