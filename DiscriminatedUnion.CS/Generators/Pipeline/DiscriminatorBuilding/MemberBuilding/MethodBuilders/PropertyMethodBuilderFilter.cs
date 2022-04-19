using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class PropertyMethodBuilderFilter : MethodBuilderBase
{
    protected override MethodMemberBuilderResponse BuildMemberSyntaxComponent(
        MemberBuilderContext<IMethodSymbol> context)
    {
        var symbol = context.MemberSymbol;

        IEnumerable<IPropertySymbol> propertyMembers = context.Discriminator.WrappedTypeSymbol
            .GetMembers()
            .OfType<IPropertySymbol>();

        if (propertyMembers.Any(p => symbol.EqualsDefault(p.GetMethod) || symbol.EqualsDefault(p.SetMethod)))
            return new MethodMemberBuilderResponse(MethodMemberBuilderResult.Invalid, context.TypeDeclarationSyntax);

        return new MethodMemberBuilderResponse(MethodMemberBuilderResult.NotBuilt, context.TypeDeclarationSyntax);
    }
}