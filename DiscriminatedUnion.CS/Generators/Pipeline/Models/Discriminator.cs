using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct Discriminator(
    INamedTypeSymbol WrappedTypeSymbol,
    SimpleNameSyntax WrappedTypeName,
    SimpleNameSyntax Name);