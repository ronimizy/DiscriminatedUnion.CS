using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;

public record struct MethodMemberBuilderResponse(
    MethodMemberBuilderResult Result,
    TypeDeclarationSyntax Syntax);