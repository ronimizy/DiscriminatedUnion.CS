using DiscriminatedUnion.Generators.Generators.Models;
using DiscriminatedUnion.Generators.Utility;

namespace DiscriminatedUnion.Generators.Generators.SourceComponents.Components
{
    public class FieldComponent : ISourceComponent
    {
        private readonly ComponentModifiers _modifiers;
        private readonly string _type;
        private readonly string _name;

        public FieldComponent(ComponentModifiers modifiers, string type, string name)
        {
            _modifiers = modifiers;
            _type = type;
            _name = name;
        }

        public bool TryAddComponent(ISourceComponent component) => false;

        public bool IsCompatibleWith(ISourceComponent component)
            => component is ClassComponent;

        public void Accept(SyntaxBuilder builder)
        {
            builder.AppendLine($"{_modifiers} {_type} {_name};");
        }
    }
}