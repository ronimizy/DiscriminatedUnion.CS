using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Models;

public record struct UnionType(INamedTypeSymbol Symbol, SimpleNameSyntax Name);