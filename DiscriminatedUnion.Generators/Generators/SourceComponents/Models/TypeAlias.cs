namespace DiscriminatedUnion.Generators.Generators.SourceComponents.Models
{
    public struct TypeAlias
    {
        public TypeAlias(string name, string type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public string Type { get; }

        public void Deconstruct(out string name, out string type)
            => (name, type) = (Name, Type);
    }
}