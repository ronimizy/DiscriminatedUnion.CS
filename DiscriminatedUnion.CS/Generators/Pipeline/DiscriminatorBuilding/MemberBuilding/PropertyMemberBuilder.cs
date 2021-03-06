using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class PropertyMemberBuilder : MemberBuilderBase<IPropertySymbol>
{
    private static readonly SyntaxToken[] PropertyModifiers =
    {
        Token(SyntaxKind.PublicKeyword)
    };
    
    protected override TypeDeclarationSyntax BuildMemberDeclarationSyntaxProtected(
        MemberBuilderContext<IPropertySymbol> context)
    {
        var (symbol, fieldName, typeDeclarationSyntax) = context;
        var typeNameSyntax = symbol.Type.ToNameSyntax(fullyQualified: true);
        var property = PropertyDeclaration(typeNameSyntax, Identifier(symbol.Name)).AddModifiers(PropertyModifiers);

        var memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            fieldName,
            IdentifierName(symbol.Name));

        if (symbol.GetMethod is not null)
        {
            var accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(memberAccess))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            property = property.AddAccessorListAccessors(accessor);
        }

        if (symbol.SetMethod is not null && !symbol.SetMethod.IsInitOnly)
        {
            var accessor = AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression, memberAccess, IdentifierName("value"))))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            property = property.AddAccessorListAccessors(accessor);
        }

        return typeDeclarationSyntax.AddMembers(property);
    }
}