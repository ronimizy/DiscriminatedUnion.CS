using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline;

public interface IUnionBuilder
{
    TypeDeclarationSyntax BuildUnionTypeSyntax(UnionBuildingContext context);
    IUnionBuilder AddNext(IUnionBuilder next);
}