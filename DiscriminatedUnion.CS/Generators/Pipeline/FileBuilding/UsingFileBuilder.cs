using System.Linq;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents.Decorators;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.FileBuilding
{
    public class UsingFileBuilder : FileBuilderBase
    {
        protected override FileBuildingContext BuildFileBase(FileBuildingContext context)
        {
            var (syntax, _, component) = context;
            var usingComponent = new UsingComponentDecorator(component);
            context = context.WithComponent(usingComponent);

            var directives = syntax.SyntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>();

            foreach (var directive in directives)
            {
                if (directive.Alias is null)
                {
                    usingComponent.AddUsedNamespace(directive.Name.ToString());
                }
                else
                {
                    usingComponent.AddUsingAlias(directive.Alias.Name.ToString(), directive.Name.ToString());
                }
            }

            return context;
        }
    }
}