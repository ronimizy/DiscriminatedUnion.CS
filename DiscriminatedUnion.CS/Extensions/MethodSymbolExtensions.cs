using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

public static class MethodSymbolExtensions
{
    public static MethodDeclarationSyntax ToMethodDeclarationSyntax(this IMethodSymbol symbol)
    {
        TypeSyntax returnType = symbol.ReturnsVoid
            ? PredefinedType(Token(SyntaxKind.VoidKeyword))
            : IdentifierName(symbol.ReturnType.GetFullyQualifiedName());

        IEnumerable<ParameterSyntax> parameters = symbol.Parameters.ToParameterSyntax();
        SyntaxToken[] modifiers = GetModifiers(symbol);

        return MethodDeclaration(returnType, Identifier(symbol.Name))
            .WithModifiers(symbol.DeclaredAccessibility.ToSyntaxTokenList())
            .AddModifiers(modifiers)
            .WithParameterList(ParameterList(SeparatedList(parameters)));
    }

    private static SyntaxToken[] GetModifiers(IMethodSymbol symbol)
    {
        var modifiers = new List<SyntaxToken>();

        if (symbol.IsAbstract)
        {
            modifiers.Add(Token(SyntaxKind.AbstractKeyword));
        }

        if (symbol.IsOverride)
        {
            modifiers.Add(Token(SyntaxKind.OverrideKeyword));
        }

        if (symbol.IsVirtual)
        {
            modifiers.Add(Token(SyntaxKind.VirtualKeyword));
        }

        if (symbol.IsStatic)
        {
            modifiers.Add(Token(SyntaxKind.StaticKeyword));
        }

        if (symbol.IsSealed)
        {
            modifiers.Add(Token(SyntaxKind.SealedKeyword));
        }

        if (symbol.IsAsync)
        {
            modifiers.Add(Token(SyntaxKind.AsyncKeyword));
        }

        return modifiers.ToArray();
    }
}