using System.Collections.Immutable;

namespace DiscriminatedUnion.Generators.Generators.Models
{
    public readonly struct Keyword
    {
        private readonly string _value;

        private Keyword(string value)
        {
            _value = value;
        }

        public static Keyword Static => new Keyword("static");
        public static Keyword Abstract => new Keyword("abstract");
        public static Keyword Sealed => new Keyword("sealed");
        public static Keyword Partial => new Keyword("partial");
        public static Keyword Readonly => new Keyword("readonly");
        public static Keyword Const => new Keyword("const");
        public static Keyword Virtual => new Keyword("virtual");

        public static implicit operator ImmutableArray<Keyword>(Keyword keyword)
            => ImmutableArray.Create(keyword);

        public override string ToString()
            => _value;
    }
}