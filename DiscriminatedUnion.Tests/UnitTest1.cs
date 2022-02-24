using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using DiscriminatedUnion.Generators.Generators;
using DiscriminatedUnion.Generators.Suppressors;
using DiscriminatedUnion.Generators.Utility;
using DiscriminatedUnion.Tests.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace DiscriminatedUnion.Tests
{
    [TestFixture]
    public class GeneratorTest
    {
        [Test]
        public void SimpleGeneratorTest()
        {
            const string userSource = @"
using System;
using DiscriminatedUnion;
using System.Globalization;

namespace Test 
{
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

    public abstract partial class Result : IUnionWith<Success>, IUnionWith<Error> { }

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
            
            System.Console.WriteLine(outputMessage);
        }

        public static Result GetRoot(double value)
        {
            return value switch
            {
                < 0 => Result.Error.Create(""Value cannot be less than zero""),
                _ => Result.Success.Create(Math.Sqrt(value))
            };
        }
    }
}
";

            var comp = CompilationBuilder
                .Build(userSource, typeof(object), typeof(IUnionWith<>))
                .GetAwaiter()
                .GetResult()
                .AddReferences(MetadataReference.CreateFromFile("/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.2/System.Runtime.dll"))
                .AddReferences(MetadataReference.CreateFromFile("/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.2/System.Console.dll"));

            var newComp = RunGenerators(comp, out _, new DiscriminatedUnionSourceGenerator());

            var newFile = newComp.SyntaxTrees
                .Single(t => Path.GetFileName(t.FilePath).EndsWith(Definer.FilenameSuffix));
            var generatedText = newFile.GetText().ToString();

            Console.WriteLine(generatedText);

            var generatedComp = newComp
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(new ExhaustiveSwitchSuppressor()));

            var diagnostics = generatedComp
                .GetAllDiagnosticsAsync()
                .GetAwaiter()
                .GetResult()
                .Where(d => d.Id.Equals(ExhaustiveSwitchSuppressor.Id));
            
            Assert.IsTrue(diagnostics.All(d => d.IsSuppressed));
        }

        private static GeneratorDriver CreateDriver(params ISourceGenerator[] generators)
            => CSharpGeneratorDriver.Create(generators);

        private static Compilation RunGenerators(
            Compilation compilation, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators)
        {
            CreateDriver(generators)
                .RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out diagnostics);
            return newCompilation;
        }
    }
}