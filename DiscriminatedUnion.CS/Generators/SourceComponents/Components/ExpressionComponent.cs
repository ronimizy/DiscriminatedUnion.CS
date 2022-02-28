using DiscriminatedUnion.CS.Utility;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components
{
    public class ExpressionComponent : ISourceComponent
    {
        private readonly string _expression;

        public ExpressionComponent(string expression)
        {
            _expression = expression;
        }

        public static implicit operator ExpressionComponent(string expression)
            => new ExpressionComponent(expression);

        public bool TryAddComponent(ISourceComponent component) => false;

        public bool IsCompatibleWith(ISourceComponent component)
            => component is IExpressionContainerSourceComponent;

        public void Accept(SyntaxBuilder builder)
            => builder.AppendLine(_expression);

        public void Accept(ISourceComponentVisitor visitor) { }
    }
}