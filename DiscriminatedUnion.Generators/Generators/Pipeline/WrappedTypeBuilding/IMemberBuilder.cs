using DiscriminatedUnion.Generators.Generators.Models;
using DiscriminatedUnion.Generators.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Generators.Pipeline.WrappedTypeBuilding
{
    public interface IMemberBuilder
    {
        bool TryBuildMemberSyntaxComponent(MemberBuildingContext<ISymbol> context, out ISourceComponent? memberSyntax);
        IMemberBuilder AddNext(IMemberBuilder builder);
    }
}