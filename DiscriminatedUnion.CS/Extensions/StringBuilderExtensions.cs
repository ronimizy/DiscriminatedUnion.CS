using System.Text;

namespace DiscriminatedUnion.CS.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendJoin<T>(this StringBuilder stringBuilder, string separator, IEnumerable<T> values)
    {
        using IEnumerator<T> en = values.GetEnumerator();

        if (!en.MoveNext())
        {
            return stringBuilder;
        }

        T value = en.Current;
        if (value != null)
        {
            stringBuilder.Append(value);
        }

        while (en.MoveNext())
        {
            stringBuilder.Append(separator);
            value = en.Current;
            if (value != null)
            {
                stringBuilder.Append(value);
            }
        }

        return stringBuilder;
    }
}