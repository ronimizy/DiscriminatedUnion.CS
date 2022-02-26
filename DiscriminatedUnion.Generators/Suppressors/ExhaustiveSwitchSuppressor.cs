using System.Collections.Immutable;
using System.Linq;
using DiscriminatedUnion.Generators.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace DiscriminatedUnion.Generators.Suppressors
{
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
            var unionInterface = context.Compilation.GetTypeByMetadataName("DiscriminatedUnion.IUnionWith`1");

            if (unionInterface is null)
                return;

            foreach (var diagnostic in context.ReportedDiagnostics)
            {
                ProcessDiagnostic(context, diagnostic, unionInterface);
            }
        }

        private static void ProcessDiagnostic(
            SuppressionAnalysisContext context,
            Diagnostic diagnostic,
            INamedTypeSymbol unionInterface)
        {
            var tree = diagnostic.Location.SourceTree!;
            var model = context.GetSemanticModel(tree);
            var node = tree.GetRoot().FindNode(diagnostic.Location.SourceSpan);
            var operation = model.GetOperation(node) as ISwitchExpressionOperation;

            if (operation is null)
                return;

            var type = operation.Value.Type;

            if (type is null)
                return;

            var wrappedTypes = type.AllInterfaces
                .Where(t => t.DerivesOrConstructedFrom(unionInterface))
                .Select(t => t.TypeArguments.Single())
                .ToImmutableArray();

            if (wrappedTypes.Length is 0)
                return;

            var matchedTypes = operation.Descendants()
                .OfType<IDeclarationPatternOperation>()
                .Select(o => o.MatchedType)
                .OfType<INamedTypeSymbol>()
                .SelectMany(t => t.GetAttributes())
                .SelectMany(a => a.ConstructorArguments)
                .Select(c => c.Value)
                .OfType<INamedTypeSymbol>();

            if (!wrappedTypes.All(t => matchedTypes.Any(tt => tt.EqualsDefault(t))))
                return;
            
            context.ReportSuppression(Suppression.Create(Descriptor, diagnostic));
        }
    }
}