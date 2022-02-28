using System.Collections.Immutable;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Models
{
    public readonly struct ComponentModifiers
    {
        private readonly Accessibility _accessibility;
        private readonly ImmutableArray<Keyword> _keywords;

        public ComponentModifiers(Accessibility accessibility)
            : this(accessibility, ImmutableArray<Keyword>.Empty) { }

        public ComponentModifiers(Accessibility accessibility, ImmutableArray<Keyword> keywords)
        {
            _accessibility = accessibility;
            _keywords = keywords;
        }

        public override string ToString()
        {
            var builder = new StringBuilder(_accessibility.ToFormattedString());

            foreach (var keyword in _keywords)
            {
                builder.Append($" {keyword}");
            }

            return builder.ToString();
        }
    }
}