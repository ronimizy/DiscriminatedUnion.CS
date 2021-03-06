using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class FieldBuilder : BuilderBase
{
    private static readonly SyntaxTokenList Modifiers = TokenList(new[]
    {
        Token(SyntaxKind.PrivateKeyword),
        Token(SyntaxKind.ReadOnlyKeyword)
    });
    
    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(DiscriminatorTypeBuildingContext context)
    {
        var variableDeclaration = VariableDeclaration(context.Discriminator.WrappedTypeName)
            .WithVariables(SingletonSeparatedList(VariableDeclarator(context.FieldName.Identifier)));

        var field = FieldDeclaration(variableDeclaration).WithModifiers(Modifiers);
        return context.TypeDeclaration.AddMembers(field);
    }
}