using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding;

public class ConstructorBuilder : WrappedTypeBuilderBase
{
    protected override void BuildWrappedTypePrivate(WrappedTypeBuildingContext context)
    {
        var attributes = new ComponentModifiers(Accessibility.Public);
        var ctorArgument = new Argument($"{context.WrappedTypeName}", "value");
        var constructorSyntax = new ConstructorComponent(attributes, context.DiscriminatorSymbol.Name, ctorArgument)
        {
            new ExpressionComponent("_value = value;"),
        };

        context.Component.AddComponentOrThrow(constructorSyntax);
    }
}