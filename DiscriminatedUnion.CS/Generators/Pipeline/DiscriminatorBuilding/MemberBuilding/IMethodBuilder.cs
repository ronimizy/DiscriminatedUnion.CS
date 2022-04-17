using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding;

public interface IMethodBuilder
{
    MethodMemberBuilderResponse TryBuildMemberSyntaxComponent(MemberBuilderContext<IMethodSymbol> context);
    IMethodBuilder AddNext(IMethodBuilder builder);
}