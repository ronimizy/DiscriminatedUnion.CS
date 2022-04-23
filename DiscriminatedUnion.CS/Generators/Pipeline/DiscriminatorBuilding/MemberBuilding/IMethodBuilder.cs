using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public interface IMethodBuilder : IResponsibilityChainLink<IMethodBuilder>
{
    MethodMemberBuilderResponse TryBuildMemberSyntaxComponent(MemberBuilderContext<IMethodSymbol> context);
}