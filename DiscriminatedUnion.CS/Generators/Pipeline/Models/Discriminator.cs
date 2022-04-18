using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct Discriminator(
    INamedTypeSymbol WrappedTypeSymbol,
    string WrappedTypeName,
    string Name);