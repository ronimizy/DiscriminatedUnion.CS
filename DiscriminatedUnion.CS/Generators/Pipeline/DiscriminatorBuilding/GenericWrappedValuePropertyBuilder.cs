using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class GenericWrappedValuePropertyBuilder : DiscriminatorBuilderBase
{
    private static readonly SyntaxToken[] Modifiers =
    {
        Token(SyntaxKind.PublicKeyword),
    };
    
    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(DiscriminatorTypeBuildingContext context)
    {
        var (declaration, _, discriminator, fieldName) = context;

        if (discriminator.WrappedTypeSymbol is not ITypeParameterSymbol)
            return declaration;

        var propertyName = discriminator.WrappedTypeName.ToString() switch
        {
            "T" => "Value",
            var name when name.StartsWith("T") => name.Substring(1),
            var name => name,
        };

        var propertyDeclaration = PropertyDeclaration(discriminator.WrappedTypeName, propertyName)
            .AddModifiers(Modifiers)
            .WithExpressionBody(ArrowExpressionClause(fieldName))
            .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));

        return declaration.AddMembers(propertyDeclaration);
    }
}