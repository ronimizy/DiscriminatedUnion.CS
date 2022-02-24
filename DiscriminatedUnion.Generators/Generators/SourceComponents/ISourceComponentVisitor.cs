using DiscriminatedUnion.Generators.Generators.SourceComponents.Decorators;

namespace DiscriminatedUnion.Generators.Generators.SourceComponents
{
    public interface ISourceComponentVisitor
    {
        void VisitUsingComponent(UsingComponentDecorator component);
    }
}