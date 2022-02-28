using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders
{
    public class PropertyMemberBuilder : MemberBuilderBase<IPropertySymbol>
    {
        protected override bool BuildMemberSyntaxComponent(MemberBuildingContext<IPropertySymbol> context, out ISourceComponent memberSource)
        {
            var (symbol, name) = context;
            var attributes = new ComponentModifiers(symbol.DeclaredAccessibility);
            var typeName = symbol.Type.GetFullyQualifiedName();

            var propertySyntax = new PropertyComponent(attributes, typeName, symbol.Name);

            if (symbol.GetMethod is not null)
            {
                propertySyntax.Getter = new[]
                {
                    new ExpressionComponent($"return {name}.{symbol.Name};"),
                };
            }

            if (symbol.SetMethod is not null)
            {
                propertySyntax.Setter = new[]
                {
                    new ExpressionComponent($"{name}.{symbol.Name} = value;"),
                };
            }

            memberSource = propertySyntax;
            return true;
        }
    }
}