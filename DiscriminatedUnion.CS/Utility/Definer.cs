using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Utility;

public static class Definer
{
    public const string AnnotationNamespace = "DiscriminatedUnion.CS.Annotations";
    public const string DiscriminatedUnionAttributeName = "GeneratedDiscriminatedUnionAttribute";
    public const string DiscriminatedUnionAttributeFullyQualifiedName = $"{AnnotationNamespace}.{DiscriminatedUnionAttributeName}";
    public const string DiscriminatorInterfaceName = "IDiscriminator";
    public const string DiscriminatorInterfaceFullyQualifiedName = $"{AnnotationNamespace}.{DiscriminatorInterfaceName}`1";
    public const string FilenameSuffix = ".DiscriminatedUnion.cs";

    public static TypeAlias MakeWrappedTypeAlias(INamedTypeSymbol symbol)
        => new TypeAlias($"Wrapped{symbol.Name}", symbol.GetFullyQualifiedName());
}