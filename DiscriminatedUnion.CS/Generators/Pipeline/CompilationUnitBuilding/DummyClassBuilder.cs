using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.CompilationUnitBuilding;

public class DummyClassBuilder : CompilationUnitBuilderBase
{
    private static readonly IEqualityComparer<NonGeneratedDiscriminator> EqualityComparer =
        EqualityComparerFactory.Create<NonGeneratedDiscriminator>(
            (a, b) => a.Name.Identifier.Equals(b.Name.Identifier));

    protected override CompilationUnitSyntax BuildCompilationUnitSyntaxProtected(CompilationUnitBuildingContext context)
    {
        var (syntax, unionType, discriminators) = context;

        MemberDeclarationSyntax[] nonGeneratedNamedDiscriminators = discriminators
            .OfType<NonGeneratedDiscriminator>()
            .Distinct(EqualityComparer)
            .Select(ToClassDeclaration)
            .Select(s => (MemberDeclarationSyntax)s)
            .ToArray();

        if (nonGeneratedNamedDiscriminators.Length is 0)
            return syntax;

        var ns = NamespaceDeclaration(unionType.Symbol.ContainingNamespace.ToNameSyntax(true))
            .AddMembers(nonGeneratedNamedDiscriminators);

        return syntax.AddMembers(ns);
    }

    private static ClassDeclarationSyntax ToClassDeclaration(NonGeneratedDiscriminator discriminator)
    {
        var constructor = ConstructorDeclaration(discriminator.Name.Identifier)
            .AddModifiers(Token(SyntaxKind.PrivateKeyword))
            .AddBodyStatements();

        return ClassDeclaration(discriminator.Name.Identifier)
            .AddModifiers(Token(SyntaxKind.InternalKeyword), Token(SyntaxKind.SealedKeyword))
            .AddMembers(constructor);
    }
}