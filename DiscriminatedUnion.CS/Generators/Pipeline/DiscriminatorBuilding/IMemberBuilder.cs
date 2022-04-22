using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public interface IMemberBuilder : IResponsibilityChainLink<IMemberBuilder>
{
    TypeDeclarationSyntax BuildMemberDeclarationSyntax(MemberBuilderContext<ISymbol> context);
}