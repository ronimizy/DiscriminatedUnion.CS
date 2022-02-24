using System.Collections.Generic;
using System.Linq;
using DiscriminatedUnion.Generators.Generators.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Extensions
{
    public static class ParameterSymbolExtensions
    {
        public static Argument[] ToArguments(this IEnumerable<IParameterSymbol> parameterSymbols)
            => parameterSymbols
                .Select(p => new Argument(p.Type.GetFullyQualifiedName(), p.Name))
                .ToArray();
    }
}