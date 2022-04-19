using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class ConversionBuilder : UnionBuilderBase
{
    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(
        UnionBuildingContext context)
    {
        var (syntax, unionType, discriminators) = context;

        MemberDeclarationSyntax[] conversions = discriminators
            .Select(d => CreateConversion(unionType, d))
            .ToArray();

        return syntax.AddMembers(conversions);
    }

    private static MemberDeclarationSyntax CreateConversion(UnionType unionType, Discriminator discriminator)
    {
        const string parameterName = "value";

        var typeAssessExpression = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            unionType.Name, discriminator.Name);

        var memberAccess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            typeAssessExpression, IdentifierName("Create"));

        var invocation = InvocationExpression(memberAccess)
            .AddArgumentListArguments(Argument(IdentifierName(parameterName)));

        return discriminator.WrappedTypeName
            .ToConversion(unionType.Name, Identifier(parameterName))
            .AddBodyStatements(ReturnStatement(invocation));
    }
}