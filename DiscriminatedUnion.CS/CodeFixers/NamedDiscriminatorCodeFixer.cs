using System.Collections.Immutable;
using DiscriminatedUnion.CS.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.CodeFixers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(NamedDiscriminatorCodeFixer))]
public class NamedDiscriminatorCodeFixer : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds { get; } =
        ImmutableArray.Create(NamedDiscriminatorAnalyzer.Id);

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false);

        if (root?.FindNode(context.Span) is not BaseTypeSyntax declaration)
            return;

        foreach (var diagnostic in context.Diagnostics)
        {
            var codeAction = CodeAction.Create(
                $"[{NamedDiscriminatorAnalyzer.Id}] Unqualify naming type.",
                c => GetFixedDocument(context.Document, declaration, c));

            context.RegisterCodeFix(codeAction, diagnostic);
        }
    }

    private static async Task<Document> GetFixedDocument(Document document, BaseTypeSyntax declaration, CancellationToken cancellationToken)
    {
        if (declaration.Type is not GenericNameSyntax genericName)
            return document;
        
        if (genericName.TypeArgumentList.Arguments[1] is not QualifiedNameSyntax namingType)
            return document;
        
        var unqualified = namingType.Right;

        var root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
        var newRoot = root.ReplaceNode(namingType, unqualified);

        return document.WithSyntaxRoot(newRoot);
    }
}