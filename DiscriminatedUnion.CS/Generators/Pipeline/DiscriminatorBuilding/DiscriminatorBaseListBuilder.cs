using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class DiscriminatorBaseListBuilder : DiscriminatorBuilderBase
{
    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(
        DiscriminatorTypeBuildingContext context)
    {
        var baseType = SimpleBaseType(context.UnionType.Symbol.ToNameSyntax());
        var discriminator = SimpleBaseType(GenericName(Definer.DiscriminatorInterfaceName)
            .AddTypeArgumentListArguments(context.Discriminator.WrappedTypeName));

        BaseTypeSyntax[] bases = 
        {
            baseType,
            discriminator
        };

        return context.TypeDeclaration.WithBaseList(BaseList(SeparatedList(bases)));
    }
}