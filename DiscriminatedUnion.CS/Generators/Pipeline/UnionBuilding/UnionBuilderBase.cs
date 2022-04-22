using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public abstract class UnionBuilderBase : IUnionBuilder
{
    protected IUnionBuilder? Next;
    
    public TypeDeclarationSyntax BuildUnionTypeSyntax(UnionBuildingContext context)
    {
        context = context with
        {
            TypeDeclarationSyntax = BuildDiscriminatorTypeDeclarationSyntaxProtected(context),
        };

        return Next?.BuildUnionTypeSyntax(context) ?? context.TypeDeclarationSyntax;
    }

    public IUnionBuilder AddNext(IUnionBuilder link)
    {
        if (Next is null)
            Next = link;
        else
            Next.AddNext(link);

        return this;
    }

    protected abstract TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(UnionBuildingContext context);
}