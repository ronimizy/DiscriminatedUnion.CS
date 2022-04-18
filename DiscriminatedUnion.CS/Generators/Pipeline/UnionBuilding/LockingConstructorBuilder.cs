using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class LockingConstructorBuilder : UnionBuilderBase
{
    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(
        UnionBuildingContext context)
    {
        var constructor = ConstructorDeclaration(Identifier(context.UnionName))
            .AddModifiers(Token(SyntaxKind.PrivateKeyword))
            .WithBody(Block());

        return context.TypeDeclarationSyntax.AddMembers(constructor);
    }
}