using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components;

public class ClassComponent : SourceComponentWithBody
{
    private readonly ComponentModifiers _modifiers;
    private readonly string _name;
    private readonly ImmutableArray<TypeArgument> _typeArguments;
    private readonly IReadOnlyCollection<string> _inheritors;

    public ClassComponent(
        ComponentModifiers modifiers,
        string name,
        params string[] inheritors)
    {
        _modifiers = modifiers;
        _name = name;
        _typeArguments = ImmutableArray<TypeArgument>.Empty;
        _inheritors = inheritors;
    }

    public ClassComponent(
        ComponentModifiers modifiers,
        string name,
        ImmutableArray<TypeArgument> typeArguments,
        params string[] inheritors)
    {
        _modifiers = modifiers;
        _name = name;
        _typeArguments = typeArguments;
        _inheritors = inheritors;
    }

    public override bool IsCompatibleWith(ISourceComponent component)
        => component is NamespaceComponent or ClassComponent;

    protected override string GetTitle()
    {
        var builder = new StringBuilder($"{_modifiers} class {_name}");

        builder.AppendTypeParameters(_typeArguments);

        if (_inheritors.Count is not 0)
        {
            builder.Append(" : ");
            builder.AppendJoin(", ", _inheritors);
        }

        builder.AppendTypeParameterConstraints(_typeArguments);

        return builder.ToString();
    }
}