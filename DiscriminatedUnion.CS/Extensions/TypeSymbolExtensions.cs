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

    public static SimpleNameSyntax ToNameSyntax(this ITypeSymbol symbol, bool fullyQualified = false)
    {
        IdentifierNameSyntax[] typeParameters = symbol switch
        {
            INamedTypeSymbol namedTypeSymbol => namedTypeSymbol.TypeArguments
                .Select(t => IdentifierName(t.Name)).ToArray(),

            _ => Array.Empty<IdentifierNameSyntax>(),
        };

        var name = fullyQualified ? symbol.GetFullyQualifiedName() : symbol.Name;

        return typeParameters.Length is 0
            ? IdentifierName(name)
            : GenericName(Identifier(name), TypeArgumentList(SeparatedList<TypeSyntax>(typeParameters)));
    }

    public static TypeSyntax ToTypeArgumentSyntax(this ITypeSymbol symbol)
        => IdentifierName(symbol.Name);

    public static IEnumerable<TypeSyntax> ToTypeArgumentSyntax(this IEnumerable<ITypeSymbol> symbols)
        => symbols.Select(ToTypeArgumentSyntax);
}