using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Factories;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class StaticMethodBuilder : MethodBuilderBase
{
    private static readonly SyntaxToken[] IgnoredModifiers =
    {
        Token(SyntaxKind.AbstractKeyword),
    };

    private readonly MethodDeclarationFactory _methodDeclarationFactory;

    public StaticMethodBuilder(MethodDeclarationFactory methodDeclarationFactory)
    {
        _methodDeclarationFactory = methodDeclarationFactory;
    }

    protected override MethodMemberBuilderResponse BuildMemberSyntaxComponent(
        MemberBuilderContext<IMethodSymbol> context)
    {
        var (symbol, _, syntax) = context;

        if (!symbol.IsStatic)
            return new MethodMemberBuilderResponse(MethodMemberBuilderResult.NotBuilt, syntax);

        IEnumerable<ArgumentSyntax> arguments = symbol.Parameters.ToArgumentSyntax();

        var invocation = _methodDeclarationFactory
            .BuildMethodInvocationExpression(symbol, context.Discriminator.WrappedTypeName, arguments);

        StatementSyntax call = symbol.ReturnsVoid
            ? ExpressionStatement(invocation)
            : ReturnStatement(invocation);

        var method = _methodDeclarationFactory.BuildMethodDeclarationSyntax(symbol, IgnoredModifiers)
            .WithBody(Block(call));

        return new MethodMemberBuilderResponse(MethodMemberBuilderResult.Built, syntax.AddMembers(method));
    }
}