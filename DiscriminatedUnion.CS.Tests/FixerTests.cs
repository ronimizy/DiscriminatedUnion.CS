using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscriminatedUnion.CS.Analyzers;
using DiscriminatedUnion.CS.Annotations;
using DiscriminatedUnion.CS.CodeFixers;
using DiscriminatedUnion.CS.Tests.Tools;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;

namespace DiscriminatedUnion.CS.Tests;

[TestFixture]
public class FixerTests
{
    [Test]
    public async Task RequirementsCodeFixerTest()
    {
        const string invalidSource = @"
using DiscriminatedUnion.CS.Annotations;

[GeneratedDiscriminatedUnion]
public class A : IDiscriminator<int>, IDiscriminator<char> { }
";

        const string validSource = @"
using DiscriminatedUnion.CS.Annotations;

[GeneratedDiscriminatedUnion]
public abstract partial class A : IDiscriminator<int>, IDiscriminator<char> { }
";

        var referencedTypes = new[]
        {
            typeof(object),
            typeof(Console),
            typeof(GeneratedDiscriminatedUnionAttribute),
        };

        var updatedSource = await CodeFixApplier.GetUpdatedSourceTextAsync(invalidSource, referencedTypes,
            new DiscriminatedUnionBaseRequirementsAnalyzer(), new DiscriminatedUnionBaseRequirementsCodeFixer());

        var updateTree = CSharpSyntaxTree.ParseText(updatedSource);
        var validTree = CSharpSyntaxTree.ParseText(validSource);

        var updatedClassDeclaration = GetClassDeclaration(updateTree);
        var validClassDeclaration = GetClassDeclaration(validTree);

        Console.WriteLine(updatedClassDeclaration);
        Console.WriteLine(validClassDeclaration);

        Assert.IsTrue(validClassDeclaration.IsEquivalentTo(updatedClassDeclaration));
    }

    private static ClassDeclarationSyntax GetClassDeclaration(SyntaxTree tree)
    {
        var root = tree.GetRoot();
        return root.DescendantNodes().OfType<ClassDeclarationSyntax>().Single().NormalizeWhitespace();
    }
}