using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class MemberBuilder : BuilderBase
{
    private readonly IMemberBuilder _memberBuilder;

    public MemberBuilder(IEnumerable<IMemberBuilder> memberBuilders)
    {
        _memberBuilder = memberBuilders.Aggregate();
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