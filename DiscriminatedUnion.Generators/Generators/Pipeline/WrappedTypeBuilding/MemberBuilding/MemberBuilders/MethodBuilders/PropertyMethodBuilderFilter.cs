using System.Linq;
using DiscriminatedUnion.Generators.Extensions;
using DiscriminatedUnion.Generators.Generators.Models;
using DiscriminatedUnion.Generators.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders.MethodBuilders
{
    public class PropertyMethodBuilderFilter : MethodBuilderBase
    {
        protected override MethodMemberBuilderResponse BuildMemberSyntaxComponent(
            MemberBuildingContext<IMethodSymbol> context, out ISourceComponent? memberSyntax)
        {
            memberSyntax = null;
            var symbol = context.Symbol;
            var propertyMembers = context.DiscriminatorSymbol
                .GetMembers()
                .OfType<IPropertySymbol>();

            if (propertyMembers.Any(p => symbol.EqualsDefault(p.GetMethod) || symbol.EqualsDefault(p.SetMethod)))
                return MethodMemberBuilderResponse.Invalid;

            return MethodMemberBuilderResponse.NotBuilt;
        }
    }
}