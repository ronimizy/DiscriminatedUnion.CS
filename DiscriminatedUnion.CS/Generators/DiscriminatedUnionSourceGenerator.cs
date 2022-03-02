using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.Pipeline;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents.Visitors;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace DiscriminatedUnion.CS.Generators;

[Generator]
public class DiscriminatedUnionSourceGenerator : ISourceGenerator
{
    private readonly IFileBuilder _fileBuilder;
    private readonly IMemberBuilder _memberBuilder;
    private readonly IWrappedTypeBuilder _wrappedTypeBuilder;

    public DiscriminatedUnionSourceGenerator()
    {
        var collection = new ServiceCollection();
        collection.AddBuilders();

        var provider = collection.BuildServiceProvider();

        _memberBuilder = provider.GetServices<IMemberBuilder>().Aggregate((a, b) => a.AddNext(b));
        _fileBuilder = provider.GetServices<IFileBuilder>().Aggregate((a, b) => a.AddNext(b));
        _wrappedTypeBuilder = provider.GetServices<IWrappedTypeBuilder>().Aggregate((a, b) => a.AddNext(b));
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

        if (unionAttribute is null || discriminatorInterface is null)
            return;

        var receiver = context.SyntaxReceiver as SyntaxReceiver;

        if (receiver is null)
            return;

        foreach (var syntax in receiver.Nodes)
        {
            ProcessSyntaxNode(context, syntax, unionAttribute, discriminatorInterface);
        }
    }

    private void ProcessSyntaxNode(
        GeneratorExecutionContext generatorContext,
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol unionAttribute,
        INamedTypeSymbol discriminatorInterface)
    {
        var model = generatorContext.Compilation.GetSemanticModel(syntax.SyntaxTree);
        var unionType = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;

        if (unionType is null)
            return;

        ImmutableArray<WrappedType> wrappedTypes = unionType.GetTypeMembers()
            .Where(t => t.Interfaces.Any(i => i.DerivesOrConstructedFrom(discriminatorInterface)))
            .Select(t => new WrappedType(t, ExtractWrappedType(t.Interfaces, discriminatorInterface)))
            .ToImmutableArray();

        if (!wrappedTypes.Any())
            return;

        ISourceComponent componentRoot = new NamespaceComponent(unionType.ContainingNamespace.GetRealName());
        var context = new FileBuildingContext(syntax, unionType, componentRoot);
        componentRoot = _fileBuilder.BuildFile(context).Component;

        var usingSystemVisitor = new AddUsingVisitor("System", "DiscriminatedUnion.CS.Annotations");
        componentRoot.Accept(usingSystemVisitor);

        var modifiers = new ComponentModifiers(unionType.DeclaredAccessibility, Keyword.Partial);
        var unionComponent = new ClassComponent(modifiers, unionType.Name);
        componentRoot.AddComponentOrThrow(unionComponent);

        var unionName = unionType.Name;
        foreach (var wrappedType in wrappedTypes)
        {
            ProcessInterfaceImplementation(generatorContext, wrappedType, componentRoot, unionComponent, unionName);
        }

        var stringBuilder = new StringBuilder();
        var builder = new SyntaxBuilder(stringBuilder);
        componentRoot.Accept(builder);

        var hintName = $"{unionType.GetFullyQualifiedName()}{Definer.FilenameSuffix}";
        generatorContext.AddSource(hintName, stringBuilder.ToString());
    }

    private void ProcessInterfaceImplementation(
        GeneratorExecutionContext generatorContext,
        WrappedType wrappedType,
        ISourceComponent componentRoot,
        ISourceComponent unionComponent,
        params string[] inheritors)
    {
        var wrappedTypeName = wrappedType.Wrapped.GetFullyQualifiedName();
        var discriminatorTypeName = wrappedType.Discriminator.Name;

        var keywords = ImmutableArray.Create(Keyword.Sealed, Keyword.Partial);
        var modifiers = new ComponentModifiers(Accessibility.Public, keywords);

        var typeArguments = wrappedType.Discriminator.TypeParameters
            .Select(p => new TypeArgument(p.Name, ExtractTypeNames(p.ConstraintTypes)))
            .ToImmutableArray();

        var wrappedComponent = new ClassComponent(modifiers, discriminatorTypeName, typeArguments, inheritors);
        unionComponent.AddComponentOrThrow(wrappedComponent);

        const string fieldName = "_value";
        var wrappedContext = new WrappedTypeBuildingContext(
            wrappedType.Discriminator, wrappedTypeName, wrappedComponent, fieldName);
        _wrappedTypeBuilder.BuildWrappedType(wrappedContext);

        IEnumerable<ISymbol> members = wrappedType.Wrapped.GetMembers().Where(SymbolAccessibilityPredicate);

        var compilation = generatorContext.Compilation;
        foreach (var member in members)
        {
            var context = new MemberBuildingContext<ISymbol>(
                member, "_value", compilation, wrappedType.Wrapped, wrappedTypeName, typeArguments);
            if (_memberBuilder.TryBuildMemberSyntaxComponent(context, out var memberSyntax))
            {
                wrappedComponent.AddComponentOrThrow(memberSyntax!);
            }
        }
    }

    private static bool SymbolAccessibilityPredicate(ISymbol symbol)
        => symbol.DeclaredAccessibility is Accessibility.Public ||
           symbol.DeclaredAccessibility is Accessibility.Internal;

    private static IReadOnlyCollection<string> ExtractTypeNames(IReadOnlyCollection<ITypeSymbol> symbols)
        => symbols.Select(s => s.GetFullyQualifiedName()).ToList();

    private static INamedTypeSymbol ExtractWrappedType(
        ImmutableArray<INamedTypeSymbol> interfaces, INamedTypeSymbol discriminatorInterface)
    {
        return interfaces.Single(i => i.DerivesOrConstructedFrom(discriminatorInterface))
            .TypeArguments
            .OfType<INamedTypeSymbol>()
            .Single();
    }
}