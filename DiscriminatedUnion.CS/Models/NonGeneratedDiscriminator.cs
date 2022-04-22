using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Models;

public class NonGeneratedDiscriminator : Discriminator
{
    public NonGeneratedDiscriminator(
        ITypeSymbol wrappedTypeSymbol,
        SimpleNameSyntax wrappedTypeName,
        SimpleNameSyntax name)
        : base(wrappedTypeSymbol, wrappedTypeName, name) { }
}