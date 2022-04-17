using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public record struct CompilationUnitBuildingContext(
    CompilationUnitSyntax Syntax,
    INamedTypeSymbol UnionTypeSymbol,
    IReadOnlyCollection<INamedTypeSymbol> WrappedTypeSymbols);