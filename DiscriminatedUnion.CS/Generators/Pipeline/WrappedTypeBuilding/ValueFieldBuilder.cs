using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding
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