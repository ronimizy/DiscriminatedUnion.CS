using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding.MethodBuilders;

public class StaticMethodBuilder : MethodBuilderBase
{
    protected override MethodMemberBuilderResponse BuildMemberSyntaxComponent(MemberBuilderContext<IMethodSymbol> context)
    {
        var (symbol, _, syntax) = context;

        if (!symbol.IsStatic)
            return new MethodMemberBuilderResponse(MethodMemberBuilderResult.NotBuilt, syntax);
        
        IEnumerable<ArgumentSyntax> arguments = symbol.Parameters.ToArgumentSyntax();

        var invocation = symbol.ToInvocationExpressionSyntax(context.Discriminator.WrappedTypeName, arguments);

        StatementSyntax call = symbol.ReturnsVoid
            ? ExpressionStatement(invocation)
            : ReturnStatement(invocation);

        var method = symbol.ToMethodDeclarationSyntax()
            .WithBody(Block(call));

        return new MethodMemberBuilderResponse(MethodMemberBuilderResult.Built, syntax.AddMembers(method));
    }
}