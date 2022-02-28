using DiscriminatedUnion.CS.Utility;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Decorators
{
    public abstract class ComponentDecoratorBase : ISourceComponent
    {
        protected readonly ISourceComponent Wrapped;

        protected ComponentDecoratorBase(ISourceComponent wrapped)
        {
            Wrapped = wrapped;
        }

        public bool TryAddComponent(ISourceComponent component)
            => Wrapped.TryAddComponent(component);

        public bool IsCompatibleWith(ISourceComponent component)
            => Wrapped.IsCompatibleWith(component);

        public virtual void Accept(SyntaxBuilder builder)
            => Wrapped.Accept(builder);

        public virtual void Accept(ISourceComponentVisitor visitor)
            => Wrapped.Accept(visitor);
    }
}