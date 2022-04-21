using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Extensions;

public static class BaseTypeSyntaxExtensions
{
    public static bool DerivesOrConstructedFrom(this BaseTypeSyntax syntax, SemanticModel model, INamedTypeSymbol baseType)
        => model.GetTypeInfo(syntax.Type).Type is INamedTypeSymbol namedTypeSymbol &&
           namedTypeSymbol.DerivesOrConstructedFrom(baseType);
}