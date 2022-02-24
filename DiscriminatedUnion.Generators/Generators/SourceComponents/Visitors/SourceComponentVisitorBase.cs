using DiscriminatedUnion.Generators.Generators.SourceComponents.Decorators;

namespace DiscriminatedUnion.Generators.Generators.SourceComponents.Visitors
{
    public abstract class SourceComponentVisitorBase : ISourceComponentVisitor
    {
        public virtual void VisitUsingComponent(UsingComponentDecorator component) { }
    }
}