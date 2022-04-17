using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

public static class TypeParameterSymbolExtensions
{
    public static IEnumerable<TypeParameterSyntax> ToTypeParameterSyntax(this IEnumerable<ITypeParameterSymbol> symbols)
        => symbols.Select(p => TypeParameter(Identifier(p.Name)));
}