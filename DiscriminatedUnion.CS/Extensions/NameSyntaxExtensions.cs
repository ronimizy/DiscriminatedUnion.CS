using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

public static class NameSyntaxExtensions
{
    private static readonly SyntaxToken[] Modifiers =
    {
        Token(SyntaxKind.PublicKeyword),
        Token(SyntaxKind.StaticKeyword)
    };

    public static ConversionOperatorDeclarationSyntax ToConversion(
        this TypeSyntax source,
        TypeSyntax target,
        SyntaxToken parameterName)
    {
        var parameter = Parameter(parameterName).WithType(source);

        return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), target)
            .AddModifiers(Modifiers)
            .AddParameterListParameters(parameter);
    }
}