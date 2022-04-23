using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Factories;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class MethodMemberBuilder : MemberBuilderBase<IMethodSymbol>
{
    private static readonly SyntaxToken[] IgnoredModifiers =
    {
        Token(SyntaxKind.AbstractKeyword),
    };
    
    private readonly IMethodBuilder _methodBuilder;
    private readonly MethodDeclarationFactory _methodDeclarationFactory;

    public MethodMemberBuilder(IEnumerable<IMethodBuilder> methodBuilders, MethodDeclarationFactory methodDeclarationFactory)
    {
        _methodDeclarationFactory = methodDeclarationFactory;
        _methodBuilder = methodBuilders.Aggregate();
    }

    protected override TypeDeclarationSyntax BuildMemberDeclarationSyntaxProtected(
        MemberBuilderContext<IMethodSymbol> context)
    {
        var response = _methodBuilder.TryBuildMemberSyntaxComponent(context);

        return response switch
        {
            { Result: MethodMemberBuilderResult.Built } r => r.Syntax,
            { Result: MethodMemberBuilderResult.NotBuilt } => BuildMemberSyntaxComponent(context),
            { Result: MethodMemberBuilderResult.Invalid } => context.TypeDeclarationSyntax,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    protected TypeDeclarationSyntax BuildMemberSyntaxComponent(MemberBuilderContext<IMethodSymbol> context)
    {
        var (symbol, fieldName, syntax) = context;
        IEnumerable<ArgumentSyntax> arguments = symbol.Parameters.ToArgumentSyntax();

        var invocation = _methodDeclarationFactory.BuildMethodInvocationExpression(symbol, fieldName, arguments);

        StatementSyntax call = symbol.ReturnsVoid
            ? ExpressionStatement(invocation)
            : ReturnStatement(invocation);

        var method = _methodDeclarationFactory.BuildMethodDeclarationSyntax(symbol, IgnoredModifiers)
            .WithBody(Block(call));

        return syntax.AddMembers(method);
    }
}