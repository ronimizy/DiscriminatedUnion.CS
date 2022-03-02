using System.Collections.Immutable;
using System.Linq;
using System.Text;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;

namespace DiscriminatedUnion.CS.Extensions;

public static class TypeArgumentExtensions
{
    public static StringBuilder AppendTypeParameters(this StringBuilder builder, ImmutableArray<TypeArgument> arguments)
    {
        if (arguments.Length is not 0)
        {
            builder.Append('<');
            builder.AppendJoin(", ", arguments.Select(a => a.Argument));
            builder.Append('>');
        }

        return builder;
    }

    public static StringBuilder AppendTypeParameterConstraints(
        this StringBuilder builder, ImmutableArray<TypeArgument> arguments)
    {
        arguments = arguments
            .Where(a => a.Constraints.Count is not 0)
            .ToImmutableArray();

        if (arguments.Length is not 0)
        {
            builder.Append(' ');

            foreach (var (type, constraints) in arguments)
            {
                builder.Append("where ");
                builder.Append(type);
                builder.Append(" : ");
                builder.AppendJoin(", ", constraints);
                builder.Append(' ');
            }
        }

        return builder;
    }
}