using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline;

public interface ICompilationUnitBuilder
{
    CompilationUnitSyntax BuildCompilationUnitSyntax(CompilationUnitBuildingContext context);
    ICompilationUnitBuilder AddNext(ICompilationUnitBuilder builder);
}