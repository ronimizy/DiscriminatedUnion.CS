using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

public static class MethodSymbolExtensions
{
    private static readonly TypeSyntax VoidTypeSyntax = PredefinedType(Token(SyntaxKind.VoidKeyword));

    public static MethodDeclarationSyntax ToMethodDeclarationSyntax(this IMethodSymbol symbol)
    {
        TypeSyntax returnType = symbol.ReturnsVoid
            ? VoidTypeSyntax
            : symbol.ReturnType.ToNameSyntax(fullyQualified: true);

        IEnumerable<ParameterSyntax> parameters = symbol.Parameters.ToParameterSyntax();
        SyntaxToken[] modifiers = GetModifiers(symbol);

        TypeParameterSyntax[] typeParameters = symbol.TypeParameters
            .ToTypeParameterSyntax()
            .ToArray();

        var declaration = MethodDeclaration(returnType, Identifier(symbol.Name))
            .WithModifiers(symbol.DeclaredAccessibility.ToSyntaxTokenList())
            .AddModifiers(modifiers)
            .WithParameterList(ParameterList(SeparatedList(parameters)));

        if (typeParameters.Length is not 0)
        {
            declaration = declaration.AddTypeParameterListParameters(typeParameters);
        }

        return declaration;
    }

    public static InvocationExpressionSyntax ToInvocationExpressionSyntax(
        this IMethodSymbol symbol,
        ExpressionSyntax identifier,
        IEnumerable<ArgumentSyntax> arguments)

    {
        TypeSyntax[] typeArguments = symbol.TypeParameters
            .ToTypeArgumentSyntax()
            .Select(t => (TypeSyntax)t)
            .ToArray();

        SimpleNameSyntax name = typeArguments.Length is 0
            ? IdentifierName(symbol.Name)
            : GenericName(symbol.Name).AddTypeArgumentListArguments(typeArguments);

        var invocation = InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, identifier, name))
            .WithArgumentList(ArgumentList(SeparatedList(arguments)));

        return invocation;
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