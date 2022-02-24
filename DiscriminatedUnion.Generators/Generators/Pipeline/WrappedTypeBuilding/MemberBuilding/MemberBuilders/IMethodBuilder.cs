using DiscriminatedUnion.Generators.Generators.Models;
using DiscriminatedUnion.Generators.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders
{
    public interface IMethodBuilder
    {
        MethodMemberBuilderResponse TryBuildMemberSyntaxComponent(MemberBuildingContext<IMethodSymbol> context, out ISourceComponent? memberSyntax);
        IMethodBuilder AddNext(IMethodBuilder builder);
    }
}