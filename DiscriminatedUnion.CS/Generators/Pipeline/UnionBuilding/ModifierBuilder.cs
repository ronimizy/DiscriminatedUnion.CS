using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class ModifierBuilder : BuilderBase
{
    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(UnionBuildingContext context)
    {
        var (syntax, unionType, _) = context;

        return syntax
            .WithModifiers(unionType.Symbol.DeclaredAccessibility.ToSyntaxTokenList())
            .AddModifiers(Token(SyntaxKind.AbstractKeyword), Token(SyntaxKind.PartialKeyword));
    }
}