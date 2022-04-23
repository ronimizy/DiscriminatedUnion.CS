using DiscriminatedUnion.CS.Analyzers;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Factories;
using DiscriminatedUnion.CS.Generators.Pipeline;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace DiscriminatedUnion.CS.Generators.SourceGeneration;

[Generator]
public class DiscriminatedUnionSourceGenerator : ISourceGenerator
{
    private readonly PipelineManager _pipelineManager;

    public DiscriminatedUnionSourceGenerator()
    {
        var collection = new ServiceCollection();
        collection.AddPipelineComponents();
        collection.AddSingleton<PipelineManager>();
        collection.AddSingleton<DiscriminatorFactory>();
        collection.AddSingleton<MethodDeclarationFactory>();
        collection.AddSingleton<ConversionOperationDeclarationFactory>();

        var provider = collection.BuildServiceProvider();
        _pipelineManager = provider.GetRequiredService<PipelineManager>();
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        context.CancellationToken.ThrowIfCancellationRequested();

        var unionAttribute = context.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatedUnionAttributeFullyQualifiedName);

        var discriminatorInterface = context.Compilation
            .GetTypeByMetadataName(Definer.DiscriminatorInterfaceFullyQualifiedName);

        var namedDiscriminatorInterface = context.Compilation
            .GetTypeByMetadataName(Definer.NamedDiscriminatorInterfaceFullyQualifiedName);

        if (unionAttribute is null || discriminatorInterface is null || namedDiscriminatorInterface is null)
            return;

        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            return;

        foreach (var syntax in receiver.Nodes)
        {
            var semanticModel = context.Compilation.GetSemanticModel(syntax.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol unionTypeSymbol)
                continue;

            if (!unionTypeSymbol.GetAttributes().Any(a => unionAttribute.EqualsDefault(a.AttributeClass)))
                continue;

            if (!DiscriminatedUnionBaseRequirementsAnalyzer.IsTypeCompliant(unionTypeSymbol))
                continue;

            var pipelineManagerContext = new PipelineManagerContext(
                unionTypeSymbol,
                discriminatorInterface,
                namedDiscriminatorInterface,
                semanticModel,
                syntax,
                unionTypeSymbol,
                context.AddSource,
                context.CancellationToken);

#if RELEASE
            try
            {
#endif
                _pipelineManager.GenerateDiscriminatedUnion(pipelineManagerContext);
#if RELEASE
            }
            // Rider source generation dies for current project session, if an exception occurs during running of a source generator.
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception) { }
#endif
        }
    }
}