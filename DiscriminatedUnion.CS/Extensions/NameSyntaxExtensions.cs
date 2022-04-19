using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

public static class NameSyntaxExtensions
{
    public static ConversionOperatorDeclarationSyntax ToConversion(this TypeSyntax source, TypeSyntax target, SyntaxToken parameterName)
    {
        SyntaxToken[] modifiers =
        {
            Token(SyntaxKind.PublicKeyword),
            Token(SyntaxKind.StaticKeyword)
        };

        var parameter = Parameter(parameterName).WithType(source);

        return ConversionOperatorDeclaration(Token(SyntaxKind.ImplicitKeyword), target)
            .AddModifiers(modifiers)
            .AddParameterListParameters(parameter);
    }
}