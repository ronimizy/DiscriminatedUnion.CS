using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DiscriminatedUnion.CS.Analyzers;
using DiscriminatedUnion.CS.Generators;
using DiscriminatedUnion.CS.Suppressors;
using DiscriminatedUnion.CS.Tests.Tools;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using NUnit.Framework;

namespace DiscriminatedUnion.CS.Tests
{
    [TestFixture]
    public class GeneratorTest
    {
        [Test]
        public async Task SimpleGeneratorTest()
        {
            const string userSource = @"
using System;
using DiscriminatedUnion.CS.Annotations;
using System.Globalization;

namespace Test
{
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

        public string Message { get; }
    }

    [GeneratedDiscriminatedUnion]
    public abstract partial class Result<T> : IDiscriminator<Success<T>>, IDiscriminator<Error> { }

    [GeneratedDiscriminatedUnion]
    public abstract partial class A : IDiscriminator<int> { }

    public static class Program 
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
                < 0 => Result<double>.Error.Create(""Value cannot be less than zero""),
                _ => Result<double>.Success.Create(Math.Sqrt(value))
            };
        }
    }
}
";

            var comp = (await CompilationBuilder
                    .Build(userSource, typeof(object), typeof(Annotations.GeneratedDiscriminatedUnionAttribute)))
                .AddReferences(MetadataReference.CreateFromFile(
                    "/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.2/System.Runtime.dll"))
                .AddReferences(MetadataReference.CreateFromFile(
                    "/usr/local/share/dotnet/shared/Microsoft.NETCore.App/6.0.2/System.Console.dll"));

            var newComp = RunGenerators(comp, out _, new DiscriminatedUnionSourceGenerator());

            IEnumerable<SyntaxTree> newFiles = newComp.SyntaxTrees
                .Where(t => Path.GetFileName(t.FilePath).EndsWith(Definer.FilenameSuffix));

            var newFileTexts = newFiles.Select(t => t.GetText().ToString());

            foreach (var file in newFileTexts)
            {
                Console.WriteLine(file);
                Console.WriteLine(new string('-', 30));
            }


            var generatedComp = newComp
                .WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(
                    new ExhaustiveSwitchSuppressor(),
                    new DiscriminatedUnionBaseRequirementsAnalyzer()));

            var diagnostics = await generatedComp.GetAllDiagnosticsAsync();
            var switchDiagnostics = diagnostics
                .Where(d => d.Id.Equals(ExhaustiveSwitchSuppressor.SuppressedDiagnosticId)).ToImmutableArray();

            Assert.IsTrue(switchDiagnostics.All(d => d.IsSuppressed));
            Assert.IsFalse(diagnostics.Except(switchDiagnostics).Any(d => d.Severity is DiagnosticSeverity.Error));
        }

        private static GeneratorDriver CreateDriver(params ISourceGenerator[] generators)
            => CSharpGeneratorDriver.Create(generators);

        private static Compilation RunGenerators(
            Compilation compilation,
            out ImmutableArray<Diagnostic> diagnostics,
            params ISourceGenerator[] generators)
        {
            CreateDriver(generators)
                .RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out diagnostics);
            return newCompilation;
        }
    }
}