// See https://aka.ms/new-console-template for more information

using System;
using System.Globalization;
using DiscriminatedUnion.CS.Annotations;

namespace DiscriminatedUnion.CS.Sample;

[GeneratedDiscriminatedUnion]
public abstract partial class A : IDiscriminator<int>, IDiscriminator<char> { }

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

    public static A B()
    {
        return 1;
    }
}