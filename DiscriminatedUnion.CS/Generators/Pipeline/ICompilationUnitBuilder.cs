using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline;

public interface ICompilationUnitBuilder : IResponsibilityChainLink<ICompilationUnitBuilder>
{
    CompilationUnitSyntax BuildCompilationUnitSyntax(CompilationUnitBuildingContext context);
}