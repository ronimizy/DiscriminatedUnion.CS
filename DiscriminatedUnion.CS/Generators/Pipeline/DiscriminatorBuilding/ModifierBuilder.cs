using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class ModifierBuilder : BuilderBase
{
    private static readonly SyntaxTokenList Modifiers = TokenList(new[]
    {
        Token(SyntaxKind.PublicKeyword),
        Token(SyntaxKind.SealedKeyword),
    });
    
    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(DiscriminatorTypeBuildingContext context) 
        => context.TypeDeclaration.WithModifiers(Modifiers);
}