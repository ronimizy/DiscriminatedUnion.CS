using System;
using System.Collections.Generic;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Utility;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components;

public class PropertyComponent : ISourceComponent
{
    private readonly ComponentModifiers _modifiers;
    private readonly string _type;
    private readonly string _name;

    public PropertyComponent(ComponentModifiers modifiers, string type, string name)
    {
        _modifiers = modifiers;
        _type = type;
        _name = name;
    }

    public IReadOnlyCollection<ExpressionComponent>? Getter { get; set; }
    public IReadOnlyCollection<ExpressionComponent>? Setter { get; set; }

    public bool TryAddComponent(ISourceComponent component) => false;

    public bool IsCompatibleWith(ISourceComponent component)
        => component is ClassComponent;

    public void Accept(SyntaxBuilder builder)
    {
        if (Getter is null && Setter is null)
            throw new InvalidOperationException("Property must have at least one accessor specified");

        builder.AppendLine($"{_modifiers} {_type} {_name}");
        using var body = builder.OpenBraces();

        if (Getter is not null)
        {
            body.AppendLine("get");
            using var getter = body.OpenBraces();
            foreach (var component in Getter)
            {
                component.Accept(getter);
            }
        }

        if (Setter is not null)
        {
            body.AppendLine("set");
            using var setter = body.OpenBraces();
            foreach (var component in Setter)
            {
                component.Accept(setter);
            }
        }
    }

    public void Accept(ISourceComponentVisitor visitor) { }
}