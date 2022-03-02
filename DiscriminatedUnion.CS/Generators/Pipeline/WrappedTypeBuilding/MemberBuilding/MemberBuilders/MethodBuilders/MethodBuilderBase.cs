using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders.MethodBuilders;

public abstract class MethodBuilderBase : IMethodBuilder
{
    protected IMethodBuilder? Next;

    public MethodMemberBuilderResponse TryBuildMemberSyntaxComponent(
        MemberBuildingContext<IMethodSymbol> context, out ISourceComponent? memberSyntax)
    {
        var response = BuildMemberSyntaxComponent(context, out memberSyntax);

        if (response is MethodMemberBuilderResponse.NotBuilt && Next is not null)
            return Next.TryBuildMemberSyntaxComponent(context, out memberSyntax);

        return response;
    }

    public IMethodBuilder AddNext(IMethodBuilder builder)
    {
        if (Next is null)
            Next = builder;
        else
            Next.AddNext(builder);

        return this;
    }

    protected abstract MethodMemberBuilderResponse BuildMemberSyntaxComponent(
        MemberBuildingContext<IMethodSymbol> context, out ISourceComponent? memberSyntax);
}