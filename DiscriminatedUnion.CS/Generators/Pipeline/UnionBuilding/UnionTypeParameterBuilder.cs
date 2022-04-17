using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class UnionTypeParameterBuilder : UnionBuilderBase
{
    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(UnionBuildingContext context)
    {
        var (declarationSyntax, unionTypeSymbol, _) = context;
        TypeParameterSyntax[] typeParameters = unionTypeSymbol.TypeParameters
            .ToTypeParameterSyntax()
            .ToArray();

        return typeParameters.Length is 0
            ? declarationSyntax
            : declarationSyntax.AddTypeParameterListParameters(typeParameters.ToArray());
    }
}