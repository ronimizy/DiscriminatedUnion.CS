using System.Collections.Immutable;
using System.Composition;
using DiscriminatedUnion.CS.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.CodeFixers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(DiscriminatedUnionBaseRequirementsCodeFixer))]
[Shared]
public class DiscriminatedUnionBaseRequirementsCodeFixer : CodeFixProvider
{
    private static readonly SyntaxToken[] Modifiers =
    {
        Token(SyntaxKind.AbstractKeyword),
        Token(SyntaxKind.PartialKeyword),
    };

    public const string Id = "DU1000";

    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(Id);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root?.FindNode(context.Span) is not ClassDeclarationSyntax declaration)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var codeAction = CodeAction.Create(
                "Add missing Discriminated Union modifiers.",
                c => GetFixedDocument(context.Document, declaration, c));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }

    private static async Task<Document> GetFixedDocument(
        Document document,
        ClassDeclarationSyntax declarationSyntax,
        CancellationToken cancellationToken)
    {
        var semanticModel = (await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false))!;
        var symbol = semanticModel.GetDeclaredSymbol(declarationSyntax)!;

        var newDeclaration = declarationSyntax
            .WithModifiers(symbol.DeclaredAccessibility.ToSyntaxTokenList())
            .AddModifiers(Modifiers)
            .NormalizeWhitespace();

        var root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
        var newRoot = root.ReplaceNode(declarationSyntax, newDeclaration);

        return document.WithSyntaxRoot(newRoot);
    }
}