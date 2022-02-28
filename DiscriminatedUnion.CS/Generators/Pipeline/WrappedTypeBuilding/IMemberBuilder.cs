using DiscriminatedUnion.CS.Generators.Models;
using DiscriminatedUnion.CS.Generators.SourceComponents;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding
{
    public interface IMemberBuilder
    {
        bool TryBuildMemberSyntaxComponent(MemberBuildingContext<ISymbol> context, out ISourceComponent? memberSyntax);
        IMemberBuilder AddNext(IMemberBuilder builder);
    }
}