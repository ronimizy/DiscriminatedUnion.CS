using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding.MethodBuilders;

public abstract class MethodBuilderBase : IMethodBuilder
{
    protected IMethodBuilder? Next;

    public MethodMemberBuilderResponse TryBuildMemberSyntaxComponent(MemberBuilderContext<IMethodSymbol> context)
    {
        var response = BuildMemberSyntaxComponent(context);

        if (response is { Result: MethodMemberBuilderResult.NotBuilt } && Next is not null)
            return Next.TryBuildMemberSyntaxComponent(context);

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
        MemberBuilderContext<IMethodSymbol> context);
}