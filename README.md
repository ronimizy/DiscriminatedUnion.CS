# Discriminated Union [![Nuget](https://img.shields.io/nuget/vpre/DiscriminatedUnion.CS?style=flat-square)](https://www.nuget.org/packages/DiscriminatedUnion.CS/0.0.1-alpha)

A library that provides functionality to define a Discriminated Union in C#.

### Define some types
```cs
public class Success
{
    public Success(double value)
    {
        Value = value;
    }

    public double Value { get; }
}

public class Error
{
    public Error(string message)
    {
        Message = message;
    }

    public string Message { get; }
}
```

### Define union type
Union type must be an abstract, partial class.

Every implementation of `IUnionWith<>` interface adds a type to union.

```cs
public abstract partial class Result : IUnionWith<Success>, IUnionWith<Error> { }
```

### Use the union type

```cs
public class Program
{
    public static void Main(string[] args)
    {
        var result = GetRoot(-1);
        var outputMessage = result switch
        {
            Result.Success s => s.Value.ToString(CultureInfo.InvariantCulture),
            Result.Error e => e.Message
        };
        
        Console.WriteLine(outputMessage);
    }   

    public static Result GetRoot(double value)
    {
        return value switch
        {
            < 0 => Result.Error.Create("Value cannot be less than zero"),
            _ => Result.Success.Create(Math.Sqrt(value))
        };
    }
}
```

## Limitations
- Generic types not supported
- Generic method not supported
- Union one type multiple times not supported
- T.b.a
