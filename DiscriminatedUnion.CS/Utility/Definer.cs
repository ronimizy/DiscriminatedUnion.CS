using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Utility;

public static class Definer
{
    public const string AnnotationNamespace = "DiscriminatedUnion.CS.Annotations";
    public const string UnionWithInterfaceName = "IUnionWith";
    public const string UnionWithInterfaceFullyQualifiedName = $"{AnnotationNamespace}.IUnionWith`1";
    public const string FilenameSuffix = ".DiscriminatedUnion.cs";

    public static TypeAlias MakeWrappedTypeAlias(INamedTypeSymbol symbol)
        => new TypeAlias($"Wrapped{symbol.Name}", symbol.GetFullyQualifiedName());
}