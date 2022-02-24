using System.Collections.Generic;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Decorators;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Models;

namespace DiscriminatedUnion.Generators.Generators.SourceComponents.Visitors
{
    public class AddAliasesVisitor : SourceComponentVisitorBase
    {
        private readonly IReadOnlyCollection<TypeAlias> _alias;

        public AddAliasesVisitor(params TypeAlias[] alias)
        {
            _alias = alias;
        }

        public override void VisitUsingComponent(UsingComponentDecorator component)
        {
            foreach (var alias in _alias)
            {
                component.AddUsingAlias(alias);
            }
        }
    }
}