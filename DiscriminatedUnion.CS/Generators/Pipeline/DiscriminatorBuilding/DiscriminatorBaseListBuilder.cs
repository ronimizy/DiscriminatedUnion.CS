using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class DiscriminatorBaseListBuilder : DiscriminatorBuilderBase
{
    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(
        DiscriminatorTypeBuildingContext context)
    {
        var baseType = SimpleBaseType(context.UnionType.Symbol.ToNameSyntax());
        return context.TypeDeclaration.WithBaseList(BaseList(SingletonSeparatedList<BaseTypeSyntax>(baseType)));
    }
}