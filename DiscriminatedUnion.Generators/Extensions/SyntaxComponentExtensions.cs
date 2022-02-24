using System;
using DiscriminatedUnion.Generators.Generators.SourceComponents;

namespace DiscriminatedUnion.Generators.Extensions
{
    public static class SyntaxComponentExtensions
    {
        public static void AddComponentOrThrow(this ISourceComponent component, ISourceComponent added)
        {
            if (!component.TryAddComponent(added))
                throw new InvalidOperationException($"Cannot add a {added} component to {component}.");
        }
    }
}