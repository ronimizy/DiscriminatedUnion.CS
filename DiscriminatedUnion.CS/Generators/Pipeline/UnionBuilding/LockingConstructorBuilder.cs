using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class LockingConstructorBuilder : BuilderBase
{
    private static readonly BlockSyntax EmptyBlock = Block();

    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(
        UnionBuildingContext context)
    {
        var constructor = ConstructorDeclaration(context.UnionType.Name.Identifier)
            .AddModifiers(Token(SyntaxKind.PrivateKeyword))
            .WithBody(EmptyBlock);

        return context.TypeDeclarationSyntax.AddMembers(constructor);
    }
}