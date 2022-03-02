using System.Linq;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders.MethodBuilders;

public class ConstructorMethodBuilder : MethodBuilderBase
{
    protected override MethodMemberBuilderResponse BuildMemberSyntaxComponent(
        MemberBuildingContext<IMethodSymbol> context, out ISourceComponent? memberSyntax)
    {
        memberSyntax = null;
        if (!context.WrappedSymbol.Constructors.Contains(context.Symbol))
            return MethodMemberBuilderResponse.NotBuilt;

        var (symbol, _) = context;
        var attributes = new ComponentModifiers(symbol.DeclaredAccessibility, Keyword.Static);
        var arguments = symbol.Parameters.ToArguments();

        var argumentNames = string.Join(", ", symbol.Parameters.Select(p => p.Name));
        var expression = $"return new {context.DiscriminatorTypeName}(new {context.WrappedTypeName}({argumentNames}));";

        memberSyntax = new MethodComponent(attributes, context.DiscriminatorTypeName, "Create", arguments)
        {
            new ExpressionComponent(expression),
        };

        return MethodMemberBuilderResponse.Built;
    }
}