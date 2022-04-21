using System.Collections.Immutable;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DiscriminatedUnion.CS.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamedDiscriminatorAnalyzer : DiagnosticAnalyzer
{
    public const string Id = "DU1010";
    public const string Title = nameof(NamedDiscriminatorAnalyzer);
    public const string Message = "If a naming type for discriminator is not exising, it must be unqualified.";

    public static DiagnosticDescriptor Descriptor { get; } = new DiagnosticDescriptor(
        Id, Title, Message, "DiscriminatedUnion", DiagnosticSeverity.Error, true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } =
        ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);

        context.RegisterCompilationStartAction(c =>
        {
            var namedDiscriminatedUnionType = c.Compilation
                .GetTypeByMetadataName(Definer.NamedDiscriminatorInterfaceFullyQualifiedName);

            if (namedDiscriminatedUnionType is null)
                return;

            c.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, SyntaxKind.ClassDeclaration);
        });
    }

    private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext obj)
    {
        obj.CancellationToken.ThrowIfCancellationRequested();

        var syntax = (ClassDeclarationSyntax)obj.Node;

        var namedDiscriminatedUnionType = obj.Compilation
            .GetTypeByMetadataName(Definer.NamedDiscriminatorInterfaceFullyQualifiedName);

        var unionAttribute = obj.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatedUnionAttributeFullyQualifiedName);

        if (namedDiscriminatedUnionType is null || unionAttribute is null)
            return;

        var semanticModel = obj.SemanticModel;
        var type = semanticModel.GetDeclaredSymbol<INamedTypeSymbol>(syntax);

        if (!type.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
            return;

        if (syntax.BaseList is null)
            return;

        IEnumerable<BaseTypeSyntax> invalidNamedDiscriminatorDeclarations = syntax.BaseList.Types
            .Where(s => s.DerivesOrConstructedFrom(semanticModel, namedDiscriminatedUnionType))
            .Where(s => AnalyzeNamedDiscriminator(s, semanticModel) is NamedDiscriminatorType.Invalid);

        foreach (var declaration in invalidNamedDiscriminatorDeclarations)
        {
            obj.ReportDiagnostic(Diagnostic.Create(Descriptor, declaration.GetLocation()));
        }
    }

    public static NamedDiscriminatorType AnalyzeNamedDiscriminator(
        BaseTypeSyntax namedDiscriminator,
        SemanticModel semanticModel)
    {
        var type = namedDiscriminator.Type;

        if (type is not GenericNameSyntax genericNameSyntax)
            return NamedDiscriminatorType.Invalid;

        var discriminatorTypeSyntax = genericNameSyntax.TypeArgumentList.Arguments[1];
        var discriminatorType = semanticModel.GetTypeInfo(discriminatorTypeSyntax).Type;

        if (discriminatorType is not IErrorTypeSymbol)
            return NamedDiscriminatorType.Exising;

        if (discriminatorTypeSyntax is QualifiedNameSyntax)
            return NamedDiscriminatorType.Invalid;

        return NamedDiscriminatorType.NonGenerated;
    }
}