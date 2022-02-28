using System.Text;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Extensions
{
    public static class NamespaceOrTypeSymbolExtensions
    {
        public static string GetFullyQualifiedName(this INamespaceSymbol symbol)
        {
            var builder = new StringBuilder(symbol.Name);
            symbol = symbol.ContainingNamespace;

            while (symbol is { IsGlobalNamespace: false })
            {
                builder.Insert(0, '.');
                builder.Insert(0, symbol.Name);
                symbol = symbol.ContainingNamespace;
            }

            return builder.ToString();
        }

        public static string GetFullyQualifiedName(this INamespaceOrTypeSymbol symbol)
        {
            var builder = new StringBuilder(symbol.Name);

            var viewedType = symbol;
            while (viewedType.ContainingType is not null)
            {
                viewedType = viewedType.ContainingType;
                builder.Insert(0, '.');
                builder.Insert(0, viewedType.Name);
            }

            var viewedNamespace = viewedType.ContainingNamespace;
            while (!viewedNamespace.IsGlobalNamespace)
            {
                builder.Insert(0, '.');
                builder.Insert(0, viewedNamespace.Name);
                viewedNamespace = viewedNamespace.ContainingNamespace;
            }

            return builder.ToString();
        }

        public static string GetRealName(this INamespaceSymbol symbol)
            => symbol.IsGlobalNamespace ? "global" : symbol.GetFullyQualifiedName();
    }
}