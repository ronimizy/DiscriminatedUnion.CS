using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class PropertyMemberBuilder : MemberBuilderBase<IPropertySymbol>
{
    protected override TypeDeclarationSyntax BuildMemberDeclarationSyntaxProtected(
        MemberBuilderContext<IPropertySymbol> context)
    {
        var (symbol, fieldName, typeDeclarationSyntax) = context;
        var typeName = symbol.Type.GetFullyQualifiedName();
        var property = PropertyDeclaration(IdentifierName(typeName), Identifier(symbol.Name))
            .AddModifiers(Token(SyntaxKind.PublicKeyword));

        var memberAccess = MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName(fieldName),
            IdentifierName(symbol.Name));

        if (symbol.GetMethod is not null)
        {
            var accessor = AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                .WithExpressionBody(ArrowExpressionClause(memberAccess))
                .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            property = property.AddAccessorListAccessors(accessor);
        }

        if (symbol.SetMethod is not null)
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