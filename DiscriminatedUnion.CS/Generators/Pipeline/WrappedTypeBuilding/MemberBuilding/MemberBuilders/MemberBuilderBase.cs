using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders
{
    public abstract class MemberBuilderBase<TSymbol> : IMemberBuilder where TSymbol : ISymbol
    {
        protected IMemberBuilder? Next;

        public virtual bool TryBuildMemberSyntaxComponent(
            MemberBuildingContext<ISymbol> context, out ISourceComponent? memberSyntax)
        {
            memberSyntax = null;
            var typedContext = context.As<TSymbol>();
            
            if (typedContext is null)
                return Next?.TryBuildMemberSyntaxComponent(context, out memberSyntax) ?? false;

            if (BuildMemberSyntaxComponent(typedContext.Value, out memberSyntax))
                return true;

            return Next?.TryBuildMemberSyntaxComponent(context, out memberSyntax) ?? false;
        }

        public IMemberBuilder AddNext(IMemberBuilder builder)
        {
            if (Next is null)
                Next = builder;
            else
                Next.AddNext(builder);

            return this;
        }

        protected abstract bool BuildMemberSyntaxComponent(
            MemberBuildingContext<TSymbol> context, out ISourceComponent memberSource);
    }
}