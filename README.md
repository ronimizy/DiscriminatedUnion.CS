# Discriminated Union [![Nuget](https://img.shields.io/nuget/vpre/DiscriminatedUnion.CS?style=flat-square)](https://www.nuget.org/packages/DiscriminatedUnion.CS/0.0.1-alpha)

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
Union type must be an abstract, partial class and have to be marked with `[GeneratedDiscriminatedUnion]` attribute.

Every discriminator type must implement `IDiscriminator<T>` interface. 
Where `T` is the type that constraints a discriminator.

```cs
[GeneratedDiscriminatedUnion]
public abstract partial class Result
{
    public partial class Success<T> : IDiscriminator<Sample.Success<T>> { }
    public partial class Error : IDiscriminator<Sample.Error> { }
}
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
            Result.Success<double> s => s.Value.ToString(CultureInfo.InvariantCulture),
            Result.Error e => e.Message,
        };

        Console.WriteLine(outputMessage);
    }

    public static Result GetRoot(double value)
    {
        return value switch
        {
            < 0 => Result.Error.Create("Value cannot be less than zero"),
            _ => Result.Success<double>.Create(Math.Sqrt(value))
        };
    }
}
```

## Limitations
- Generic method not supported
- Union one type multiple times not supported
- T.b.a
