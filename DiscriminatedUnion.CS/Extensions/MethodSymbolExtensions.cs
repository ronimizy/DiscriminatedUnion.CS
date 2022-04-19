using System.Collections.Immutable;
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
        var typeParameters = symbol.TypeParameters
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
        string identifier,
        IEnumerable<ArgumentSyntax> arguments)

    {
        var typeArguments = symbol.TypeParameters.ToTypeArgumentSyntax().ToArray();

        SimpleNameSyntax name = typeArguments.Length is 0
            ? IdentifierName(symbol.Name)
            : GenericName(symbol.Name).AddTypeArgumentListArguments(typeArguments);

        var invocation = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(identifier), name))
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