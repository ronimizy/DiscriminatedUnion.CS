using System.Collections.Generic;
using DiscriminatedUnion.CS.Generators.SourceComponents.Decorators;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Visitors
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