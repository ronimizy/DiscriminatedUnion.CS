using System.Collections.Immutable;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DiscriminatedUnion.CS.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DiscriminatedUnionBaseRequirementsAnalyzer : DiagnosticAnalyzer
{
    public const string Id = "DU1000";
    public const string Title = nameof(DiscriminatedUnionBaseRequirementsAnalyzer);

    public const string Message =
        "Class {0} must be an 'abstract partial class' in order to be a GeneratedDiscriminatedUnion.";

    public static DiagnosticDescriptor Descriptor { get; } = new DiagnosticDescriptor(
        Id, Title, Message, "DiscriminatedUnion", DiagnosticSeverity.Error, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
        = ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
    }

    private static void AnalyzeSymbol(SymbolAnalysisContext context)
    {
        var symbol = (INamedTypeSymbol)context.Symbol;

        var unionAttribute = context.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatedUnionAttributeFullyQualifiedName);

        if (unionAttribute is null)
            return;

        if (!symbol.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
            return;

        if (IsTypeCompliant(symbol))
            return;

        var name = symbol.ToNameSyntax(fullyQualified: true).ToString();

        foreach (var location in symbol.Locations)
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, location, name));
        }
    }

    public static bool IsTypeCompliant(INamedTypeSymbol symbol)
    {
        if (!symbol.IsAbstract || !symbol.IsReferenceType)
            return false;

        return symbol.Locations
            .Select(l => (l.SourceTree, l.SourceSpan))
            .Where(p => p.SourceTree is not null)
            .Select(p => p.SourceTree!.GetRoot().FindNode(p.SourceSpan))
            .OfType<ClassDeclarationSyntax>()
            .SelectMany(d => d.Modifiers)
            .Any(m => m.Kind() is SyntaxKind.PartialKeyword);
    }
}