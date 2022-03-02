using DiscriminatedUnion.CS.Generators.SourceComponents.Decorators;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Visitors;

public abstract class SourceComponentVisitorBase : ISourceComponentVisitor
{
    public virtual void VisitUsingComponent(UsingComponentDecorator component) { }
}