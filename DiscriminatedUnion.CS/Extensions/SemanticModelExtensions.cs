using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Extensions;

public static class SemanticModelExtensions
{
    public static T GetDeclaredSymbol<T>(this SemanticModel semanticModel, SyntaxNode node) where T : class, ISymbol
    {
        return semanticModel.GetDeclaredSymbol(node) as T ?? throw new InvalidCastException();
    }
}