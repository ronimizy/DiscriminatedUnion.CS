using DiscriminatedUnion.CS.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.Models;

public record struct MemberBuilderContext<TSymbol>(
    TypeDeclarationSyntax TypeDeclarationSyntax,
    TSymbol MemberSymbol,
    Discriminator Discriminator,
    IdentifierNameSyntax FieldName)
    where TSymbol : ISymbol
{
    public void Deconstruct(out TSymbol memberSymbol, out IdentifierNameSyntax fieldName, out TypeDeclarationSyntax typeDeclarationSyntax)
    {
        (memberSymbol, fieldName, typeDeclarationSyntax) = (MemberSymbol, FieldName, TypeDeclarationSyntax);
    }

    public MemberBuilderContext<T>? As<T>() where T : ISymbol
    {
        if (MemberSymbol is T symbol)
        {
            return new MemberBuilderContext<T>(
                TypeDeclarationSyntax,
                symbol,
                Discriminator,
                FieldName);
        }

        return null;
    }
}