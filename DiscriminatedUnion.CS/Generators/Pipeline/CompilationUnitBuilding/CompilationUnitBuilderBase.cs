using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.CompilationUnitBuilding;

public abstract class CompilationUnitBuilderBase : ICompilationUnitBuilder
{
    protected ICompilationUnitBuilder? Next;

    public CompilationUnitSyntax BuildCompilationUnitSyntax(CompilationUnitBuildingContext context)
    {
        context = context with
        {
            Syntax = BuildCompilationUnitSyntaxProtected(context),
        };

        return Next?.BuildCompilationUnitSyntax(context) ?? context.Syntax;
    }

    public ICompilationUnitBuilder AddNext(ICompilationUnitBuilder builder)
    {
        if (Next is null)
            Next = builder;
        else
            Next.AddNext(builder);

        return this;
    }

    protected abstract CompilationUnitSyntax
        BuildCompilationUnitSyntaxProtected(CompilationUnitBuildingContext context);
}