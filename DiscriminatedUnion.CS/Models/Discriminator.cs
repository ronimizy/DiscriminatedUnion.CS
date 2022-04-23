using DiscriminatedUnion.CS.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Models;

public record Discriminator(
    ITypeSymbol WrappedTypeSymbol,
    SimpleNameSyntax WrappedTypeName,
    SimpleNameSyntax Name)
{
    public static Discriminator Create(ITypeSymbol wrappedTypeSymbol)
    {
        return new Discriminator(wrappedTypeSymbol,
            wrappedTypeSymbol.ToNameSyntax(fullyQualified: true),
            IdentifierName(wrappedTypeSymbol.Name));
    }
}