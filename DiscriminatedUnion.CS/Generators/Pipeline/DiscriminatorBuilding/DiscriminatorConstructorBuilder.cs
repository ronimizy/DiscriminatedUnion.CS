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
        var parameter = Parameter(Identifier(valueParameterName)).WithType(IdentifierName(discriminator.WrappedTypeName));

        var assignmentExpression = AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            IdentifierName(context.FieldName),
            IdentifierName(valueParameterName));

        var constructor = ConstructorDeclaration(Identifier(discriminator.Name))
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddParameterListParameters(parameter)
            .WithBody(Block(SingletonList<StatementSyntax>(ExpressionStatement(assignmentExpression))));

        return context.TypeDeclaration.AddMembers(constructor);
    }
}