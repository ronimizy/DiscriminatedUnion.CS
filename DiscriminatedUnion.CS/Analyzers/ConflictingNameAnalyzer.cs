using System.Collections.Immutable;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConflictingNameAnalyzer : DiagnosticAnalyzer
{
    public const string Id = "DU1020";
    public const string Title = nameof(ConflictingNameAnalyzer);
    public const string Message = "Ambiguous discriminator name. Consider changing naming type.";

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

            c.RegisterSyntaxNodeAction(ProcessNode, SyntaxKind.ClassDeclaration);
        });
    }

    private static void ProcessNode(SyntaxNodeAnalysisContext obj)
    {
        var discriminatedUnionType = obj.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatorInterfaceFullyQualifiedName);

        var namedDiscriminatedUnionType = obj.Compilation
            .GetTypeByMetadataName(Definer.NamedDiscriminatorInterfaceFullyQualifiedName);

        var unionAttribute = obj.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatedUnionAttributeFullyQualifiedName);

        if (discriminatedUnionType is null || namedDiscriminatedUnionType is null || unionAttribute is null)
            return;

        var semanticModel = obj.SemanticModel;
        var syntax = (ClassDeclarationSyntax)obj.Node;
        var type = semanticModel.GetDeclaredSymbol<INamedTypeSymbol>(syntax);

        if (!type.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
            return;

        if (syntax.BaseList is null)
            return;

        bool IsDiscriminator(BaseTypeSyntax t)
            => t.DerivesOrConstructedFrom(semanticModel, discriminatedUnionType) ||
               t.DerivesOrConstructedFrom(semanticModel, namedDiscriminatedUnionType);

        BaseTypeSyntax[] baseSyntax = syntax.BaseList.Types
            .Where(IsDiscriminator)
            .ToArray();

        IdentifierNameSyntax[] names = baseSyntax
            .Select(s => s.Type)
            .Select(t => semanticModel.GetTypeInfo(t).Type)
            .OfType<INamedTypeSymbol>()
            .Select(t => t.TypeArguments.Last())
            .Select(t => IdentifierName(t.Name))
            .ToArray();

        NameSyntax[] ambiguousNames = GetAmbiguouslyNamedTypes(syntax, names).ToArray();

        IEnumerable<TypeSyntax> ambiguousBases = baseSyntax
            .Select(b => b.Type)
            .OfType<GenericNameSyntax>()
            .Select(g => g.TypeArgumentList.Arguments.Last())
            .Where(t => ambiguousNames.Any(n => n.ToString().Equals(semanticModel.GetTypeInfo(t).Type?.Name)));

        foreach (var baseTypeSyntax in ambiguousBases)
        {
            obj.ReportDiagnostic(Diagnostic.Create(Descriptor, baseTypeSyntax.GetLocation()));
        }
    }

    public static IEnumerable<NameSyntax> GetAmbiguouslyNamedTypes(
        ClassDeclarationSyntax unionDeclarationSyntax,
        IReadOnlyCollection<NameSyntax> symbols)
    {
        var parameters = unionDeclarationSyntax.TypeParameterList?.Parameters
            .Select(p => p.Identifier.ToString()).ToArray() ?? Array.Empty<string>();

        return symbols.Where(s
            => symbols.Count(ss => ss.IsEquivalentTo(s)) > 1 || parameters.Any(p => s.ToString().Equals(p)));
    }
}