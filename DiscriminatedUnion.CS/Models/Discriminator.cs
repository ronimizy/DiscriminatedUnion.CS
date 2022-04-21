using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Models;

public class Discriminator
{
    public Discriminator(
        INamedTypeSymbol wrappedTypeSymbol,
        SimpleNameSyntax wrappedTypeName,
        SimpleNameSyntax name)
    {
        WrappedTypeSymbol = wrappedTypeSymbol;
        WrappedTypeName = wrappedTypeName;
        Name = name;
    }

    public INamedTypeSymbol WrappedTypeSymbol { get; }
    public SimpleNameSyntax WrappedTypeName { get; }
    public SimpleNameSyntax Name { get; }
}