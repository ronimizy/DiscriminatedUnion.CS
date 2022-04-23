using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class TypeParameterBuilder : BuilderBase
{
    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(UnionBuildingContext context)
    {
        var (declarationSyntax, unionType, _) = context;
        TypeParameterSyntax[] typeParameters = unionType.Symbol.TypeParameters
            .ToTypeParameterSyntax()
            .ToArray();

        return typeParameters.Length is 0
            ? declarationSyntax
            : declarationSyntax.AddTypeParameterListParameters(typeParameters.ToArray());
    }
}