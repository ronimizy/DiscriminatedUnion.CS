using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Components;

public class MethodComponent : SourceComponentWithBody, IExpressionContainerSourceComponent
{
    private readonly ComponentModifiers _modifiers;
    private readonly string _returnType;
    private readonly string _name;
    private readonly IReadOnlyCollection<Argument> _arguments;
    private readonly ImmutableArray<TypeArgument> _typeArguments;

    private readonly List<ExpressionComponent> _expressionSyntaxComponents =
        new List<ExpressionComponent>();

    public MethodComponent(
        ComponentModifiers modifiers,
        string returnType,
        string name,
        params Argument[] arguments)
    {
        _modifiers = modifiers;
        _returnType = returnType;
        _name = name;
        _arguments = arguments;
        _typeArguments = ImmutableArray<TypeArgument>.Empty;
    }

    public MethodComponent(
        ComponentModifiers modifiers,
        string returnType,
        string name,
        ImmutableArray<TypeArgument> typeArguments,
        params Argument[] arguments)
    {
        _modifiers = modifiers;
        _returnType = returnType;
        _name = name;
        _arguments = arguments;
        _typeArguments = typeArguments;
    }

    public override bool IsCompatibleWith(ISourceComponent component)
        => component is ClassComponent;

    protected override string GetTitle()
    {
        var builder = new StringBuilder(_modifiers.ToString());
        builder.Append($" {_returnType}");
        builder.Append($" {_name}");
        
        builder.AppendTypeParameters(_typeArguments);
        
        builder.Append('(');
        builder.AppendJoin(", ", _arguments);
        builder.Append(')');

        builder.AppendTypeParameterConstraints(_typeArguments);

        return builder.ToString();
    }

    public IEnumerator<ExpressionComponent> GetEnumerator()
        => _expressionSyntaxComponents.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Add(ExpressionComponent component)
    {
        _expressionSyntaxComponents.Add(component);
        TryAddComponent(component);
    }
}