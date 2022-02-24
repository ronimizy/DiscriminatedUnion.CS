using DiscriminatedUnion.Generators.Extensions;
using DiscriminatedUnion.Generators.Generators.Models;
using DiscriminatedUnion.Generators.Generators.Pipeline.Models;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Components;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Generators.Pipeline.WrappedTypeBuilding
{
    public class ValueFieldBuilder : WrappedTypeBuilderBase
    {
        protected override void BuildWrappedTypePrivate(WrappedTypeBuildingContext context)
        {
            var attributes = new ComponentModifiers(Accessibility.Private, Keyword.Readonly);
            var valueFieldSyntax = new FieldComponent(attributes, $"{context.Alias.Name}", "_value");
            context.Component.AddComponentOrThrow(valueFieldSyntax);
        }
    }
}