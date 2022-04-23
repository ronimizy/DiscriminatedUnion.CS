using DiscriminatedUnion.CS.Generators.Factories;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class ConversionBuilder : BuilderBase
{
    private const string ParameterName = "value";
    private static readonly IdentifierNameSyntax ParameterIdentifierName = IdentifierName(ParameterName);
    private static readonly ArgumentSyntax ParameterArgument = Argument(ParameterIdentifierName);

    private static readonly IdentifierNameSyntax CreateMethodIdentifierName = IdentifierName("Create");

    private readonly ConversionOperationDeclarationFactory _conversionFactory;

    public ConversionBuilder(ConversionOperationDeclarationFactory conversionFactory)
    {
        _conversionFactory = conversionFactory;
    }

    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(
        UnionBuildingContext context)
    {
        var (syntax, unionType, discriminators) = context;

        MemberDeclarationSyntax[] conversions = discriminators
            .Where(d => discriminators.Count(dd => d.WrappedTypeName.IsEquivalentTo(dd.WrappedTypeName)) == 1)
            .Select(d => CreateConversion(unionType, d))
            .ToArray();

        return syntax.AddMembers(conversions);
    }

    private MemberDeclarationSyntax CreateConversion(UnionType unionType, Discriminator discriminator)
    {
        var typeAssessExpression = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            unionType.Name, discriminator.Name);

        var memberAccess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
            typeAssessExpression, CreateMethodIdentifierName);

        var invocation = InvocationExpression(memberAccess).AddArgumentListArguments(ParameterArgument);

        return _conversionFactory
            .BuildConversion(discriminator.WrappedTypeName, unionType.Name, Identifier(ParameterName))
            .AddBodyStatements(ReturnStatement(invocation));
    }
}