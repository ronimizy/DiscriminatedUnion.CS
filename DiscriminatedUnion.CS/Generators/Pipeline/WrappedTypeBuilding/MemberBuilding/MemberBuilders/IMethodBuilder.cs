using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders
{
    public interface IMethodBuilder
    {
        MethodMemberBuilderResponse TryBuildMemberSyntaxComponent(MemberBuildingContext<IMethodSymbol> context, out ISourceComponent? memberSyntax);
        IMethodBuilder AddNext(IMethodBuilder builder);
    }
}