using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline;

public interface IDiscriminatorBuilder
{
    TypeDeclarationSyntax BuildDiscriminatorTypeSyntax(DiscriminatorTypeBuildingContext context);
    IDiscriminatorBuilder AddNext(IDiscriminatorBuilder next);
}