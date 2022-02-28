using DiscriminatedUnion.CS.Generators.SourceComponents.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Models;

public readonly struct MemberBuildingContext<TSymbol>
    where TSymbol : ISymbol
{
    public MemberBuildingContext(
        TSymbol symbol, 
        string fieldName, 
        Compilation compilation, 
        INamedTypeSymbol discriminatorSymbol,
        TypeAlias wrappedTypeAlias)
    {
        Symbol = symbol;
        FieldName = fieldName;
        Compilation = compilation;
        DiscriminatorSymbol = discriminatorSymbol;
        WrappedTypeAlias = wrappedTypeAlias;
    }

    public TSymbol Symbol { get; }
    public string FieldName { get; }
    public Compilation Compilation { get; }
    public INamedTypeSymbol DiscriminatorSymbol { get; }
    public TypeAlias WrappedTypeAlias { get; }

    public void Deconstruct(out TSymbol symbol, out string fieldName)
    {
        symbol = Symbol;
        fieldName = FieldName;
    }

    public MemberBuildingContext<T>? As<T>()
        where T : ISymbol
    {
        if (Symbol is T symbol)
        {
            return new MemberBuildingContext<T>(symbol, FieldName, Compilation, DiscriminatorSymbol, WrappedTypeAlias);
        }

        return null;
    }
}