namespace DiscriminatedUnion.CS.Utility;

public static class Definer
{
    public const string AnnotationNamespace = "DiscriminatedUnion.CS.Annotations";
    public const string DiscriminatedUnionAttributeName = "GeneratedDiscriminatedUnionAttribute";
    public const string DiscriminatedUnionAttributeFullyQualifiedName = $"{AnnotationNamespace}.{DiscriminatedUnionAttributeName}";
    public const string DiscriminatorInterfaceName = "IDiscriminator";
    public const string DiscriminatorInterfaceFullyQualifiedName = $"{AnnotationNamespace}.{DiscriminatorInterfaceName}`1";
    public const string NamedDiscriminatorInterfaceFullyQualifiedName = $"{AnnotationNamespace}.{DiscriminatorInterfaceName}`2";
    public const string FilenameSuffix = ".DiscriminatedUnion.cs";
}