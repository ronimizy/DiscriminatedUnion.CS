using DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public interface IMemberBuilder
{
    TypeDeclarationSyntax BuildMemberDeclarationSyntax(MemberBuilderContext<ISymbol> context);
    IMemberBuilder AddNext(IMemberBuilder builder);
}