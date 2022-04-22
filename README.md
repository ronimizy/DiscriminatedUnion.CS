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

    public void A<V>() { }

    public static void B<V>() { }
}

public class Error
{
    public Error(string message)
    {
        Message = message;
    }

    public string Message { get; init; }
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

### Custom named discriminators
To set custom names to discriminator a `naming types` are used.

Put a type you want to infer name from as a second generic argument.

```cs
[GeneratedDiscriminatedUnion]
public abstract partial class B : IDiscriminator<int>, IDiscriminator<int, OtherInt32> { }
```

If a type does not exist, it must not have any qualification (ex: wrong - `A.B`, right - `B`). If non exising type is specified correctly, a dummy class would be generated.
```cs
internal sealed class OtherInt32
{
    private OtherInt32()
    {
    }
}
```

### Generic parameter as wrapped type
Discriminator with generic wrapped type must have a `naming type` specified.

```cs
[GeneratedDiscriminatedUnion]
public abstract partial class GenericResult<T> : IDiscriminator<T, Success>, IDiscriminator<Error> { }
```

In case of a generic wrapped type, members cannot be inferred during source generation because corresponding closed types are not 
available as a source code, so discriminator would have a get-only property, containing wrapped value
(constraint interfaces implementation t.b.a.).

The wrapped value containing property name is inferred from generic parameter name according this schema:
- `T` -> `Value`
- `T*` -> `*`
- `*` -> `*`

### Limitations
 - Discriminators of same type does not have a `WrappedType -> UnionType` converter.
 - Discriminators do not implement interfaces that wrapped types are implementing.
 - Discriminators with generic wrapped types do not implement interfaces that generic parameters are constraint with.