# Discriminated Union [![Nuget](https://img.shields.io/nuget/vpre/DiscriminatedUnion.CS?style=flat-square)](https://www.nuget.org/packages/DiscriminatedUnion.CS/)

A library that provides functionality to define a Discriminated Union in C#.

### Define some types
```cs
public class Success<T>
{
    public Success(T value)
    {
        Value = value;
    }

    public T Value { get; }
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
Union type must be an abstract, partial class marked with `[GeneratedDiscriminatedUnion]` attribute.

Every implementation of `IDiscriminator<T>` defines a discriminator inside the union type.

Where `T` is the type that constraints a discriminator.

```cs
[GeneratedDiscriminatedUnion]
public abstract partial class Result<T> : IDiscriminator<Success<T>>, IDiscriminator<Error> { }
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
            Result<double>.Success s => s.Value.ToString(CultureInfo.InvariantCulture),
            Result<double>.Error e => e.Message,
        };

        Console.WriteLine(outputMessage);
    }

    public static Result<double> GetRoot(double value)
    {
        return value switch
        {
            < 0 => new Error("Value cannot be less than zero"),
            _ => new Success<double>(Math.Sqrt(value)),
        };
    }
}
```

## Limitations
- Union one type multiple times not supported
- T.b.a
