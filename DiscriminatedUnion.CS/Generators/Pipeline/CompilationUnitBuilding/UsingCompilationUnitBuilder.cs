using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.CompilationUnitBuilding;

public class UsingCompilationUnitBuilder : CompilationUnitBuilderBase
{
    private static readonly IEqualityComparer<UsingDirectiveSyntax> UsingDirectiveSyntaxEqualityComparer =
        EqualityComparerFactory.Create<UsingDirectiveSyntax>((a, b) => a.IsEquivalentTo(b));

    private static readonly UsingDirectiveSyntax[] DefaultUsingDirectives =
    {
        UsingDirective(IdentifierName("System")),
        UsingDirective(IdentifierName("DiscriminatedUnion.CS.Annotations"))
    };

    protected override CompilationUnitSyntax BuildCompilationUnitSyntaxProtected(CompilationUnitBuildingContext context)
    {
        var (syntax, unionType, discriminators) = context;

        var directives = discriminators
            .SelectMany(d => GetDirectivesFromSymbol(d.WrappedTypeSymbol))
            .Concat(GetDirectivesFromSymbol(unionType.Symbol))
            .Concat(DefaultUsingDirectives)
            .Distinct(UsingDirectiveSyntaxEqualityComparer);

        return syntax.WithUsings(List(directives));
    }

    private static IEnumerable<UsingDirectiveSyntax> GetDirectivesFromSymbol(ITypeSymbol symbol)
    {
        return symbol.Locations
            .Select(l => l.SourceTree)
            .Where(t => t is not null)
            .Select(t => t!.GetRoot())
            .SelectMany(r => r.DescendantNodes())
            .OfType<UsingDirectiveSyntax>();
    }
}