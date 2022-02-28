using DiscriminatedUnion.CS.Generators.SourceComponents.Decorators;

namespace DiscriminatedUnion.CS.Generators.SourceComponents;

public interface ISourceComponentVisitor
{
    void VisitUsingComponent(UsingComponentDecorator component);
}