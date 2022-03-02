using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Models;

public readonly struct WrappedType
{
    public WrappedType(INamedTypeSymbol discriminator, INamedTypeSymbol wrapped)
    {
        Discriminator = discriminator;
        Wrapped = wrapped;
    }

    public INamedTypeSymbol Discriminator { get; }
    public INamedTypeSymbol Wrapped { get; }
}