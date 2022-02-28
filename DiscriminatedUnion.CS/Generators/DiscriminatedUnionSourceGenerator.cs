using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.Pipeline;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using DiscriminatedUnion.CS.Generators.SourceComponents.Components;
using DiscriminatedUnion.CS.Generators.SourceComponents.Decorators;
using DiscriminatedUnion.CS.Generators.SourceComponents.Visitors;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace DiscriminatedUnion.CS.Generators
{
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

            var unionInterface = context.Compilation
                .GetTypeByMetadataName(Definer.UnionWithInterfaceFullyQualifiedName);

            if (unionInterface is null)
                return;

            var receiver = context.SyntaxReceiver as SyntaxReceiver;

            if (receiver is null)
                return;

            foreach (var syntax in receiver.Nodes)
            {
                ProcessSyntaxNode(context, syntax, unionInterface);
            }
        }

        private void ProcessSyntaxNode(
            GeneratorExecutionContext generatorContext,
            ClassDeclarationSyntax syntax,
            INamedTypeSymbol unionInterface)
        {
            var model = generatorContext.Compilation.GetSemanticModel(syntax.SyntaxTree);
            var unionType = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;

            if (unionType is null)
                return;

            INamedTypeSymbol[] wrappedTypes = unionType.Interfaces
                .Where(i => i.ConstructedFrom.EqualsDefault(unionInterface))
                .Select(t => t.TypeArguments.Single())
                .OfType<INamedTypeSymbol>()
                .ToArray();

            if (!wrappedTypes.Any())
                return;

            var stringBuilder = new StringBuilder();

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

            var builder = new SyntaxBuilder(stringBuilder);
            componentRoot.Accept(builder);

            var hintName = $"{unionType.GetFullyQualifiedName()}{Definer.FilenameSuffix}";
            generatorContext.AddSource(hintName, stringBuilder.ToString());
        }

        private void ProcessInterfaceImplementation(
            GeneratorExecutionContext generatorContext,
            INamedTypeSymbol wrappedType,
            ISourceComponent componentRoot,
            ISourceComponent unionComponent,
            params string[] inheritors)
        {
            var alias = Definer.MakeWrappedTypeAlias(wrappedType);
            var visitor = new AddAliasesVisitor(alias);
            componentRoot.Accept(visitor);

            var discriminatorTypeName = wrappedType.Name;
            var modifiers = new ComponentModifiers(Accessibility.Public, Keyword.Sealed);
            var wrappedComponent = new ClassComponent(modifiers, discriminatorTypeName, inheritors);
            var discriminatorAttribute = $"Discriminator(typeof({alias.Name}))";
            var attributedComponent = new AttributedComponentDecorator(wrappedComponent, discriminatorAttribute);
            unionComponent.AddComponentOrThrow(attributedComponent);

            const string fieldName = "_value";
            var wrappedContext = new WrappedTypeBuildingContext(wrappedType, alias, wrappedComponent, fieldName);
            _wrappedTypeBuilder.BuildWrappedType(wrappedContext);

            IEnumerable<ISymbol> members = wrappedType.GetMembers().Where(SymbolAccessibilityPredicate);

            var compilation = generatorContext.Compilation;
            foreach (var member in members)
            {
                var context = new MemberBuildingContext<ISymbol>(member, "_value", compilation, wrappedType, alias);
                if (_memberBuilder.TryBuildMemberSyntaxComponent(context, out var memberSyntax))
                {
                    wrappedComponent.AddComponentOrThrow(memberSyntax!);
                }
            }
        }

        private static bool SymbolAccessibilityPredicate(ISymbol symbol)
            => symbol.DeclaredAccessibility is Accessibility.Public ||
               symbol.DeclaredAccessibility is Accessibility.Internal;
    }
}