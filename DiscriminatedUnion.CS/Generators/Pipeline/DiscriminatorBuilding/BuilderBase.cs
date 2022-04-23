using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public abstract class BuilderBase : IDiscriminatorBuilder
{
    private IDiscriminatorBuilder? _next;

    public TypeDeclarationSyntax BuildDiscriminatorTypeSyntax(DiscriminatorTypeBuildingContext context)
    {
        context = context with
        {
            TypeDeclaration = BuildWrappedTypeDeclarationSyntaxProtected(context),
        };

        return _next?.BuildDiscriminatorTypeSyntax(context) ?? context.TypeDeclaration;
    }

    public IDiscriminatorBuilder AddNext(IDiscriminatorBuilder link)
    {
        if (_next is null)
            _next = link;
        else
            _next.AddNext(link);

        return this;
    }

    protected abstract TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(
        DiscriminatorTypeBuildingContext context);
}