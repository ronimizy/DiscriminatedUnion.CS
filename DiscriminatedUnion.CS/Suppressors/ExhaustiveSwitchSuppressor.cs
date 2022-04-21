using System.Collections.Immutable;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DiscriminatedUnion.CS.Suppressors;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ExhaustiveSwitchSuppressor : DiagnosticSuppressor
{
    public const string Id = "DU2000";
    public const string SuppressedDiagnosticId = "CS8509";

    public static readonly SuppressionDescriptor Descriptor = new SuppressionDescriptor(
        Id, SuppressedDiagnosticId, "Switch handles all possible DiscriminatedUnionInputs");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } =
        ImmutableArray.Create(Descriptor);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        var unionAttribute = context.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatedUnionAttributeFullyQualifiedName);

        var discriminatorInterface = context.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatorInterfaceFullyQualifiedName);

        if (unionAttribute is null || discriminatorInterface is null)
            return;

        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            ProcessDiagnostic(context, diagnostic, unionAttribute, discriminatorInterface);
        }
    }

    private static void ProcessDiagnostic(
        SuppressionAnalysisContext context,
        Diagnostic diagnostic,
        INamedTypeSymbol unionAttribute,
        INamedTypeSymbol discriminatorInterface)
    {
        var tree = diagnostic.Location.SourceTree!;
        var model = context.GetSemanticModel(tree);
        var node = tree.GetRoot().FindNode(diagnostic.Location.SourceSpan);
        var operation = model.GetOperation(node) as ISwitchExpressionOperation;

        var type = operation?.Value.Type;

        if (type is null)
            return;

        if (!type.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
            return;

        IEnumerable<ITypeSymbol> wrappedTypes = type.GetTypeMembers()
            .Where(t => t.Interfaces.Length is 1)
            .Where(t => t.Interfaces.Single().DerivesOrConstructedFrom(discriminatorInterface));

        var matchedTypes = operation.Descendants()
            .OfType<IDeclarationPatternOperation>()
            .Select(o => o.MatchedType)
            .OfType<INamedTypeSymbol>()
            .Where(t => t.Interfaces.Length is 1)
            .Where(t => t.Interfaces.Single().DerivesOrConstructedFrom(discriminatorInterface))
            .ToImmutableArray();

        if (!wrappedTypes.All(t => matchedTypes.Any(tt => tt.EqualsDefault(t))))
            return;

        context.ReportSuppression(Suppression.Create(Descriptor, diagnostic));
    }
}