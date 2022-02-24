using DiscriminatedUnion.Generators.Extensions;
using DiscriminatedUnion.Generators.Generators.Models;
using DiscriminatedUnion.Generators.Generators.Pipeline.Models;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Components;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Generators.Pipeline.WrappedTypeBuilding
{
    public class ConstructorBuilder : WrappedTypeBuilderBase
    {
        protected override void BuildWrappedTypePrivate(WrappedTypeBuildingContext context)
        {
            var attributes = new ComponentModifiers(Accessibility.Public);
            var ctorArgument = new Argument($"{context.Alias.Name}", "value");
            var constructorSyntax = new ConstructorComponent(attributes, context.Symbol.Name, ctorArgument)
            {
                new ExpressionComponent("_value = value;"),
            };
            context.Component.AddComponentOrThrow(constructorSyntax);
        }
    }
}