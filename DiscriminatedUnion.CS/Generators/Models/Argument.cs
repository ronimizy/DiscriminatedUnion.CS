namespace DiscriminatedUnion.CS.Generators.Models;

public readonly struct Argument
{
    private readonly string _type;
    private readonly string _name;

    public Argument(string type, string name)
    {
        _type = type;
        _name = name;
    }

    public override string ToString()
        => $"{_type} {_name}";
}