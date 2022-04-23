using System.Collections.Immutable;
using DiscriminatedUnion.CS.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Models;

public record NonGeneratedNamedDiscriminator(
        ITypeSymbol WrappedTypeSymbol,
        SimpleNameSyntax WrappedTypeName,
        SimpleNameSyntax Name)
    : Discriminator(WrappedTypeSymbol, WrappedTypeName, Name)
{
    public static NonGeneratedNamedDiscriminator Create(ImmutableArray<ITypeSymbol> typeArguments)
    {
        return new NonGeneratedNamedDiscriminator(
            typeArguments[0],
            typeArguments[0].ToNameSyntax(true),
            IdentifierName(typeArguments[1].Name));
    }
}