using System.Collections.Generic;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;

namespace DiscriminatedUnion.CS.Generators.SourceComponents;

public interface IExpressionContainerSourceComponent : ISourceComponent, IEnumerable<ExpressionComponent>
{
    void Add(ExpressionComponent component);
}