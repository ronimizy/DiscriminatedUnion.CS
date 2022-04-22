using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class ConstructorMethodBuilder : MethodBuilderBase
{
    private static readonly SyntaxToken[] MethodModifiers =
    {
        Token(SyntaxKind.StaticKeyword),
    };
    
    protected override MethodMemberBuilderResponse BuildMemberSyntaxComponent(
        MemberBuilderContext<IMethodSymbol> context)
    {
        var (memberSymbol, _, syntax) = context;
        var discriminator = context.Discriminator;

        if (discriminator.WrappedTypeSymbol is not INamedTypeSymbol namedTypeSymbol || !namedTypeSymbol.Constructors.Contains(memberSymbol))
            return new MethodMemberBuilderResponse(MethodMemberBuilderResult.NotBuilt, syntax);

        IEnumerable<ParameterSyntax> parameters = memberSymbol.Parameters.ToParameterSyntax();
        IEnumerable<ArgumentSyntax> arguments = memberSymbol.Parameters.ToArgumentSyntax();

        var wrappedCreation = ObjectCreationExpression(discriminator.WrappedTypeName)
            .WithArgumentList(ArgumentList(SeparatedList(arguments)));

        var discriminatorCreation = ObjectCreationExpression(discriminator.Name)
            .WithArgumentList(ArgumentList(SingletonSeparatedList(Argument(wrappedCreation))));

        var method = MethodDeclaration(discriminator.Name, Identifier("Create"))
            .WithModifiers(memberSymbol.DeclaredAccessibility.ToSyntaxTokenList())
            .AddModifiers(MethodModifiers)
            .WithParameterList(ParameterList(SeparatedList(parameters)))
            .WithBody(Block(SingletonList(ReturnStatement(discriminatorCreation))));

        return new MethodMemberBuilderResponse(MethodMemberBuilderResult.Built, syntax.AddMembers(method));
    }
}