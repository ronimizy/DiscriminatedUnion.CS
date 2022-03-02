// See https://aka.ms/new-console-template for more information

using System;
using System.Globalization;

namespace DiscriminatedUnion.CS.Sample;

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