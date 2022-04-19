using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct UnionType(INamedTypeSymbol Symbol, SimpleNameSyntax Name);