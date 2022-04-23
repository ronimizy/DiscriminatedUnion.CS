using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Factories;

public class ConversionOperationDeclarationFactory
{
    private static readonly SyntaxToken[] Modifiers =
    {
        Token(SyntaxKind.PublicKeyword),
        Token(SyntaxKind.StaticKeyword)
    };

    public ConversionOperatorDeclarationSyntax BuildConversion(
        TypeSyntax source,
        TypeSyntax target,
        SyntaxToken parameterName)
    {
        var parameter = Parameter(parameterName).WithType(source);

        return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), target)
            .AddModifiers(Modifiers)
            .AddParameterListParameters(parameter);
    }
}