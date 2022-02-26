using System.Linq;
using DiscriminatedUnion.Generators.Extensions;
using DiscriminatedUnion.Generators.Generators.Models;
using DiscriminatedUnion.Generators.Generators.SourceComponents;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Components;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Generators.Pipeline.WrappedTypeBuilding.MemberBuilding.MemberBuilders.MethodBuilders
{
    public class ConstructorMethodBuilder : MethodBuilderBase
    {
        protected override MethodMemberBuilderResponse BuildMemberSyntaxComponent(
            MemberBuildingContext<IMethodSymbol> context, out ISourceComponent? memberSyntax)
        {
            memberSyntax = null;
            if (!context.DiscriminatorSymbol.Constructors.Contains(context.Symbol))
                return MethodMemberBuilderResponse.NotBuilt;

            var (symbol, _) = context;
            var attributes = new ComponentModifiers(symbol.DeclaredAccessibility, Keyword.Static);
            var arguments = symbol.Parameters.ToArguments();
            var typeName = context.DiscriminatorSymbol.Name;

            var argumentNames = string.Join(", ", symbol.Parameters.Select(p => p.Name));

            memberSyntax = new MethodComponent(attributes, typeName, "Create", arguments)
            {
                new ExpressionComponent($"return new {typeName}(new {context.WrappedTypeAlias.Name}({argumentNames}));")
            };

            return MethodMemberBuilderResponse.Built;
        }
    }
}