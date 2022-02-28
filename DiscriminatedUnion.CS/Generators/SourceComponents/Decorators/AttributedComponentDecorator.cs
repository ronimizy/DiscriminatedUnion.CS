using System.Collections.Generic;
using DiscriminatedUnion.CS.Utility;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Decorators
{
    public class AttributedComponentDecorator : ComponentDecoratorBase
    {
        private readonly IReadOnlyCollection<string> _attributes;

        public AttributedComponentDecorator(ISourceComponent wrapped, params string[] attributes)
            : base(wrapped)
        {
            _attributes = attributes;
        }

        public override void Accept(SyntaxBuilder builder)
        {
            foreach (var attribute in _attributes)
            {
                builder.AppendLine($"[{attribute}]");
            }

            Wrapped.Accept(builder);
        }
    }
}