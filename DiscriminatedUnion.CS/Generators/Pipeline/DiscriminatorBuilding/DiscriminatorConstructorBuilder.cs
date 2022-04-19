using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class DiscriminatorConstructorBuilder : DiscriminatorBuilderBase
{
    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(DiscriminatorTypeBuildingContext context)
    {
        const string valueParameterName = "value";
        var discriminator = context.Discriminator;
        var parameter = Parameter(Identifier(valueParameterName)).WithType(discriminator.WrappedTypeName);

        var assignmentExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(context.FieldName),
            IdentifierName(valueParameterName));

        var constructor = ConstructorDeclaration(discriminator.Name.Identifier)
            .AddModifiers(Token(SyntaxKind.PrivateKeyword))
            .AddParameterListParameters(parameter)
            .WithBody(Block(SingletonList<StatementSyntax>(ExpressionStatement(assignmentExpression))));

        var creationExpression = ObjectCreationExpression(discriminator.Name)
            .AddArgumentListArguments(Argument(IdentifierName(valueParameterName)));

        var method = MethodDeclaration(discriminator.Name, "Create")
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword))
            .AddParameterListParameters(parameter)
            .WithBody(Block(SingletonList<StatementSyntax>(ReturnStatement(creationExpression))));

        return context.TypeDeclaration.AddMembers(constructor, method);
    }
}