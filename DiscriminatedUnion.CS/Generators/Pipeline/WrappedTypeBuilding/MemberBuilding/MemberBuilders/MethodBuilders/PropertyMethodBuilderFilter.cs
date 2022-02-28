using System.Linq;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders.MethodBuilders;

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