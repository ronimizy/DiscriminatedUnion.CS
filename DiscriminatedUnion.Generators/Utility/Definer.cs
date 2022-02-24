using DiscriminatedUnion.Generators.Extensions;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Utility
{
    public static class Definer
    {
        public const string UnionWithInterfaceName = "IUnionWith";
        public const string UnionWithInterfaceFullyQualifiedName = "DiscriminatedUnion.IUnionWith`1";
        public const string FilenameSuffix = ".DiscriminatedUnion.cs";

        public static TypeAlias MakeWrappedTypeAlias(INamedTypeSymbol symbol)
            => new TypeAlias($"Wrapped{symbol.Name}", symbol.GetFullyQualifiedName());
    }
}