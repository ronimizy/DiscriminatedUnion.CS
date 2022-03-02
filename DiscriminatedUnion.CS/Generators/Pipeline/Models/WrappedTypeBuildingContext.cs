using DiscriminatedUnion.CS.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models;

public struct WrappedTypeBuildingContext
{
    public WrappedTypeBuildingContext(
        INamedTypeSymbol discriminatorSymbol,
        string wrappedTypeName,
        ISourceComponent component,
        string fieldName)
    {
        DiscriminatorSymbol = discriminatorSymbol;
        Component = component;
        FieldName = fieldName;
        WrappedTypeName = wrappedTypeName;
    }

    public INamedTypeSymbol DiscriminatorSymbol { get; }
    public string WrappedTypeName { get; }
    public ISourceComponent Component { get; }
    public string FieldName { get; }
}