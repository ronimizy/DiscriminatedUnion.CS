using System.Collections.Generic;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components
{
    public class ClassComponent : SourceComponentWithBody
    {
        private readonly ComponentModifiers _modifiers;
        private readonly string _name;
        private readonly IReadOnlyCollection<string> _inheritors;

        public ClassComponent(ComponentModifiers modifiers, string name, params string[] inheritors)
        {
            _modifiers = modifiers;
            _name = name;
            _inheritors = inheritors;
        }

        public override bool IsCompatibleWith(ISourceComponent component)
            => component is NamespaceComponent ||
               component is ClassComponent;

        protected override string GetTitle()
        {
            var builder = new StringBuilder($"{_modifiers} class {_name}");

            if (_inheritors.Count != 0)
            {
                builder.Append(" : ");
                builder.AppendJoin(", ", _inheritors);
            }

            return builder.ToString();
        }
    }
}