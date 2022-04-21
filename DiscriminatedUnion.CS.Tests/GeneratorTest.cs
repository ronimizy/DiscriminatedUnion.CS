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
            var resultSource = new SourceFile("Result.cs", await File.ReadAllTextAsync(@"Result.cs"));
            var programSource = new SourceFile("Program.cs", await File.ReadAllTextAsync(@"Program.cs"));

            Type[] referencedTypes =
            {
                typeof(object),
                typeof(Annotations.GeneratedDiscriminatedUnionAttribute),
                typeof(Console)
            };

            var compilation = await CompilationBuilder.BuildCompilation(referencedTypes, resultSource, programSource);

            var newComp = RunGenerators(compilation, new DiscriminatedUnionSourceGenerator());

            IEnumerable<SyntaxTree> newFiles = newComp.SyntaxTrees
                .Where(t => Path.GetFileName(t.FilePath).EndsWith(Definer.FilenameSuffix));

            var newFileTexts = newFiles.Select(t => t.GetText().ToString());

            var generatedComp = newComp.WithAnalyzers(ImmutableArray.Create<DiagnosticAnalyzer>(
                new ExhaustiveSwitchSuppressor(),
                new DiscriminatedUnionBaseRequirementsAnalyzer(),
                new NamedDiscriminatorAnalyzer(),
                new ConflictingNameAnalyzer()));

            var diagnostics = await generatedComp.GetAllDiagnosticsAsync();
            var switchDiagnostics = diagnostics
                .Where(d => d.Id.Equals(ExhaustiveSwitchSuppressor.SuppressedDiagnosticId)).ToImmutableArray();

            foreach (var diagnostic in diagnostics.Except(switchDiagnostics)
                         .Where(d => d.Severity is DiagnosticSeverity.Error))
            {
                Console.WriteLine(diagnostic);
            }

            foreach (var file in newFileTexts)
            {
                Console.WriteLine(file);
                Console.WriteLine(new string('-', 30));
            }

            Assert.IsTrue(switchDiagnostics.All(d => d.IsSuppressed), "Not all diagnostics were suppressed");
            Assert.IsFalse(diagnostics.Except(switchDiagnostics).Any(d => d.Severity is DiagnosticSeverity.Error),
                "Has errors");
        }

        private static GeneratorDriver CreateDriver(params ISourceGenerator[] generators)
            => CSharpGeneratorDriver.Create(generators);

        private static Compilation RunGenerators(
            Compilation compilation,
            params ISourceGenerator[] generators)
        {
            CreateDriver(generators)
                .RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out _);
            return newCompilation;
        }
    }
}