using System.Collections.Immutable;
using DiscriminatedUnion.CS.Analyzers;
using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Factories.Models;
using DiscriminatedUnion.CS.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Factories;

public class DiscriminatorFactory
{
    public DiscriminatorFactoryBuildingResult BuildDiscriminators(DiscriminatorFactoryContext context)
    {
        var (unionTypeSyntax,
            unionTypeSymbol,
            discriminatorInterfaceSymbol,
            namedDiscriminatorInterfaceSymbol,
            semanticModel,
            cancellationToken) = context;

        cancellationToken.ThrowIfCancellationRequested();

        ILookup<NamedDiscriminatorType, BaseTypeSyntax> namedDiscriminatorInterfaces = unionTypeSyntax.BaseList!.Types
            .Where(s => s.DerivesOrConstructedFrom(semanticModel, namedDiscriminatorInterfaceSymbol))
            .ToLookup(s => NamedDiscriminatorAnalyzer.AnalyzeNamedDiscriminator(s, semanticModel));

        var hasInvalid = namedDiscriminatorInterfaces[NamedDiscriminatorType.Invalid].Any();

        var existingNamedDiscriminators = ExtractGenericArguments(
                namedDiscriminatorInterfaces[NamedDiscriminatorType.Exising],
                semanticModel,
                namedDiscriminatorInterfaceSymbol)
            .Select(ExistingNamedDiscriminator.Create);

        var nonGeneratedNamedDiscriminators = ExtractGenericArguments(
                namedDiscriminatorInterfaces[NamedDiscriminatorType.NonGenerated],
                semanticModel,
                namedDiscriminatorInterfaceSymbol)
            .Select(NonGeneratedNamedDiscriminator.Create);

        IEnumerable<Discriminator> discriminators = unionTypeSymbol.Interfaces
            .Where(i => i.DerivesOrConstructedFrom(discriminatorInterfaceSymbol))
            .Select(i => ExtractGenericArguments(i, discriminatorInterfaceSymbol))
            .Select(t => Discriminator.Create(t.Single()))
            .Concat(existingNamedDiscriminators)
            .Concat(nonGeneratedNamedDiscriminators);

        return new DiscriminatorFactoryBuildingResult(hasInvalid, discriminators);
    }

    private static IEnumerable<ImmutableArray<ITypeSymbol>> ExtractGenericArguments(
        IEnumerable<BaseTypeSyntax> baseTypes,
        SemanticModel semanticModel,
        INamedTypeSymbol derivationCeiling)
    {
        return baseTypes
            .Select(s => semanticModel.GetTypeInfo(s.Type).Type)
            .OfType<INamedTypeSymbol>()
            .Select(i => ExtractGenericArguments(i, derivationCeiling));
    }

    private static ImmutableArray<ITypeSymbol> ExtractGenericArguments(
        INamedTypeSymbol i,
        INamedTypeSymbol discriminatorInterface)
        => ExtractClosestDerivation(i, discriminatorInterface).TypeArguments;

    private static INamedTypeSymbol ExtractClosestDerivation(INamedTypeSymbol i, INamedTypeSymbol interfaceType)
    {
        while (!i.ConstructedFrom.EqualsDefault(interfaceType))
        {
            i = i.ConstructedFrom;
        }

        return i;
    }
}