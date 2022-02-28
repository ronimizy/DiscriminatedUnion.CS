using System.Collections.Generic;
using System.Linq;
using DiscriminatedUnion.CS.Utility;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components;

public abstract class SourceComponentWithBody : ISourceComponent
{
    private readonly List<ISourceComponent> _components = new List<ISourceComponent>();

    public bool TryAddComponent(ISourceComponent component)
    {
        if (!component.IsCompatibleWith(this))
            return _components.Any(syntaxComponent => syntaxComponent.TryAddComponent(component));

        _components.Add(component);
        return true;
    }

    public abstract bool IsCompatibleWith(ISourceComponent component);

    public void Accept(SyntaxBuilder builder)
    {
        builder.AppendLine(GetTitle());
        using var body = builder.OpenBraces();

        foreach (var component in _components.Take(_components.Count - 1))
        {
            component.Accept(body);
            body.AppendLine();
        }

        if (_components.Any())
        {
            _components[_components.Count - 1].Accept(body);
        }
    }

    public void Accept(ISourceComponentVisitor visitor)
    {
        _components.ForEach(c => c.Accept(visitor));
    }

    protected abstract string GetTitle();
}