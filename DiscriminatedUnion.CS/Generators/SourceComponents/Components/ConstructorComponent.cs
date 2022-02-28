using System.Collections;
using System.Collections.Generic;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components
{
    public class ConstructorComponent : SourceComponentWithBody, IExpressionContainerSourceComponent
    {
        private readonly ComponentModifiers _modifiers;
        private readonly string _type;
        private readonly IReadOnlyCollection<Argument> _arguments;
        private readonly List<ExpressionComponent> _expressionSyntaxComponents;

        public ConstructorComponent(
            ComponentModifiers modifiers, string type, params Argument[] arguments)
        {
            _modifiers = modifiers;
            _type = type;
            _arguments = arguments;
            _expressionSyntaxComponents = new List<ExpressionComponent>();
        }

        public override bool IsCompatibleWith(ISourceComponent component)
            => component is ClassComponent;

        protected override string GetTitle()
        {
            var builder = new StringBuilder(_modifiers.ToString());
            builder.Append($" {_type}");
            builder.Append('(');
            builder.AppendJoin(", ", _arguments);
            builder.Append(')');

            return builder.ToString();
        }

        public IEnumerator<ExpressionComponent> GetEnumerator()
            => _expressionSyntaxComponents.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(ExpressionComponent component)
        {
            _expressionSyntaxComponents.Add(component);
            TryAddComponent(component);
        }
    }
}