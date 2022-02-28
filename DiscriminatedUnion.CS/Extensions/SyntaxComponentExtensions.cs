using System;
using DiscriminatedUnion.CS.Generators.SourceComponents;

namespace DiscriminatedUnion.CS.Extensions;

public static class SyntaxComponentExtensions
{
    public static void AddComponentOrThrow(this ISourceComponent component, ISourceComponent added)
    {
        if (!component.TryAddComponent(added))
            throw new InvalidOperationException($"Cannot add a {added} component to {component}.");
    }
}