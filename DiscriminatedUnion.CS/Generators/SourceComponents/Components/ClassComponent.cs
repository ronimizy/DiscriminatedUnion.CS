using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components;

public class ClassComponent : SourceComponentWithBody
{
    private readonly ComponentModifiers _modifiers;
    private readonly string _name;
    private readonly IReadOnlyCollection<TypeArgument> _typeArguments;
    private readonly IReadOnlyCollection<string> _inheritors;

    public ClassComponent(
        ComponentModifiers modifiers, 
        string name, 
        IReadOnlyCollection<TypeArgument> typeArguments,
        params string[] inheritors)
    {
        _modifiers = modifiers;
        _name = name;
        _typeArguments = typeArguments;
        _inheritors = inheritors;
    }

    public override bool IsCompatibleWith(ISourceComponent component)
        => component is NamespaceComponent ||
           component is ClassComponent;

    protected override string GetTitle()
    {
        var builder = new StringBuilder($"{_modifiers} class {_name}");

        if (_typeArguments.Count is not 0)
        {
            builder.Append('<');
            builder.AppendJoin(", ", _typeArguments.Select(a => a.Argument));
            builder.Append('>');
        }

        if (_inheritors.Count is not 0)
        {
            builder.Append(" : ");
            builder.AppendJoin(", ", _inheritors);
        }

        var arguments = _typeArguments
            .Where(a => a.Constraints.Count is not 0)
            .Take(_typeArguments.Count - 1)
            .ToImmutableArray();

        if (arguments.Length is not 0)
        {
            builder.Append(' ');

            foreach (var argument in arguments)
            {
                AddConstraint(argument, builder);
                builder.Append(' ');
            }

            var a = arguments[arguments.Length - 1];
            AddConstraint(a, builder);
        }

        return builder.ToString();
    }

    private static void AddConstraint(TypeArgument argument, StringBuilder builder)
    {
        var (type, constraints) = argument;
        builder.Append("where ");
        builder.Append(type);
        builder.Append(" : ");
        builder.AppendJoin(", ", constraints);
    }
}