using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class ConstructorBuilder : BuilderBase
{
    private const string ValueParameterName = "value";
    private static readonly IdentifierNameSyntax ValueParameterIdentifierName = IdentifierName(ValueParameterName);
    private static readonly ParameterSyntax ValueParameter = Parameter(ValueParameterIdentifierName.Identifier);
    private static readonly ArgumentSyntax ValueArgument = Argument(ValueParameterIdentifierName);

    private static readonly SyntaxToken[] ConstructorModifiers = { Token(SyntaxKind.PrivateKeyword) };

    private static readonly SyntaxToken[] MethodModifiers =
    {
        Token(SyntaxKind.PublicKeyword),
        Token(SyntaxKind.StaticKeyword)
    };

    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(
        DiscriminatorTypeBuildingContext context)
    {
        var discriminator = context.Discriminator;
        var parameter = ValueParameter.WithType(discriminator.WrappedTypeName);

        var assignmentExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            context.FieldName,
            ValueParameterIdentifierName);

        var constructor = ConstructorDeclaration(discriminator.Name.Identifier)
            .AddModifiers(ConstructorModifiers)
            .AddParameterListParameters(parameter)
            .AddBodyStatements(ExpressionStatement(assignmentExpression));

        var creationExpression = ObjectCreationExpression(discriminator.Name).AddArgumentListArguments(ValueArgument);

        var method = MethodDeclaration(discriminator.Name, "Create")
            .AddModifiers(MethodModifiers)
            .AddParameterListParameters(parameter)
            .WithBody(Block(SingletonList<StatementSyntax>(ReturnStatement(creationExpression))));

        return context.TypeDeclaration.AddMembers(constructor, method);
    }
}