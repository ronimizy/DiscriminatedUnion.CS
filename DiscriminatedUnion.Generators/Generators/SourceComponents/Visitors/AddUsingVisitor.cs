using System.Collections.Generic;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Decorators;

namespace DiscriminatedUnion.Generators.Generators.SourceComponents.Visitors
{
    public class AddUsingVisitor : SourceComponentVisitorBase
    {
        private readonly IReadOnlyCollection<string> _usingStrings;

        public AddUsingVisitor(params string[] usingStrings)
        {
            _usingStrings = usingStrings;
        }

        public override void VisitUsingComponent(UsingComponentDecorator component)
        {
            foreach (var usingString in _usingStrings)
            {
                component.AddUsedNamespace(usingString);
            }
        }
    }
}