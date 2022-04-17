using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding;

public class MethodMemberBuilder : MemberBuilderBase<IMethodSymbol>
{
    private readonly IMethodBuilder _methodBuilder;

    public MethodMemberBuilder()
    {
        var collection = new ServiceCollection();

        using (var scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker)))
        {
            scanner.EnqueueAdditionOfTypesThat()
                .WouldBeRegisteredAs<IMethodBuilder>()
                .WithSingletonLifetime()
                .MustBeAssignableTo<IMethodBuilder>()
                .AreNotInterfaces()
                .AreNotAbstractClasses();
        }

        var provider = collection.BuildServiceProvider();
        _methodBuilder = provider
            .GetServices<IMethodBuilder>()
            .Aggregate((a, b) => a.AddNext(b));
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

    protected static TypeDeclarationSyntax BuildMemberSyntaxComponent(MemberBuilderContext<IMethodSymbol> context)
    {
        var (symbol, fieldName, syntax) = context;
        IEnumerable<ArgumentSyntax> arguments = symbol.Parameters.ToArgumentSyntax();

        var invocation = InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                IdentifierName(fieldName), IdentifierName(symbol.Name)))
            .WithArgumentList(ArgumentList(SeparatedList(arguments)));

        StatementSyntax call = symbol.ReturnsVoid
            ? ExpressionStatement(invocation)
            : ReturnStatement(invocation);

        var method = symbol.ToMethodDeclarationSyntax()
            .WithBody(Block(call));

        return syntax.AddMembers(method);
    }
}