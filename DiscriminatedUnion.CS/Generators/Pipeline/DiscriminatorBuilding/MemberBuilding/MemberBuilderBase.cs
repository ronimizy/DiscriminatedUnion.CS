using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding;

public abstract class MemberBuilderBase<TSymbol> : IMemberBuilder where TSymbol : ISymbol
{
    protected IMemberBuilder? Next;

    public virtual TypeDeclarationSyntax BuildMemberDeclarationSyntax(MemberBuilderContext<ISymbol> context)
    {
        MemberBuilderContext<TSymbol>? typedContext = context.As<TSymbol>();

        if (typedContext is null)
            return Next?.BuildMemberDeclarationSyntax(context) ?? context.TypeDeclarationSyntax;

        return BuildMemberDeclarationSyntaxProtected(typedContext.Value);
    }

    public IMemberBuilder AddNext(IMemberBuilder builder)
    {
        if (Next is null)
            Next = builder;
        else
            Next.AddNext(builder);

        return this;
    }

    protected abstract TypeDeclarationSyntax BuildMemberDeclarationSyntaxProtected(
        MemberBuilderContext<TSymbol> context);
}