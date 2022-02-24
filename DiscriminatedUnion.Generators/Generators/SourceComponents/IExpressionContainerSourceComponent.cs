using System.Collections.Generic;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Components;

namespace DiscriminatedUnion.Generators.Generators.SourceComponents
{
    public interface IExpressionContainerSourceComponent : ISourceComponent, IEnumerable<ExpressionComponent>
    {
        void Add(ExpressionComponent component);
    }
}