using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class DiscriminatorMemberBuilder : DiscriminatorBuilderBase
{
    private readonly IMemberBuilder _memberBuilder;

    public DiscriminatorMemberBuilder()
    {
        var collection = new ServiceCollection();

        using (var scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker)))
        {
            scanner.EnqueueAdditionOfTypesThat()
                .WouldBeRegisteredAs<IMemberBuilder>()
                .WithSingletonLifetime()
                .MustBeAssignableTo<IMemberBuilder>()
                .AreNotAbstractClasses()
                .AreNotInterfaces();
        }

        var provider = collection.BuildServiceProvider();
        _memberBuilder = provider.GetServices<IMemberBuilder>().Aggregate((a, b) => a.AddNext(b));
    }

    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(
        DiscriminatorTypeBuildingContext context)
    {
        var (typeDeclaration, _, discriminator, fieldName) = context;

        IEnumerable<ISymbol> members = discriminator.WrappedTypeSymbol.GetMembers()
            .Where(s => s.DeclaredAccessibility is Accessibility.Public or Accessibility.Internal);

        foreach (var member in members)
        {
            var memberBuilderContext = new MemberBuilderContext<ISymbol>(
                typeDeclaration,
                member,
                discriminator,
                fieldName);

            typeDeclaration = _memberBuilder.BuildMemberDeclarationSyntax(memberBuilderContext);
        }

        return typeDeclaration;
    }
}