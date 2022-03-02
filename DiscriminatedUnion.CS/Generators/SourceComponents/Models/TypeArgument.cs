using System.Collections.Generic;

namespace DiscriminatedUnion.CS.Generators.SourceComponents.Models;

public readonly struct TypeArgument
{
    public TypeArgument(string argument, IReadOnlyCollection<string> constraints)
    {
        Argument = argument;
        Constraints = constraints;
    }

    public string Argument { get; }
    public IReadOnlyCollection<string> Constraints { get; }

    public void Deconstruct(out string argument, out IReadOnlyCollection<string> constraints)
        => (argument, constraints) = (Argument, Constraints);
}