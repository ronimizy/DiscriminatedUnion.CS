using System.Collections.Immutable;
using System.Text;
using DiscriminatedUnion.CS.Extensions;
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
        INamedTypeSymbol wrappedSymbol,
        string wrappedTypeName,
        ImmutableArray<TypeArgument> typeArguments)
    {
        Symbol = symbol;
        FieldName = fieldName;
        Compilation = compilation;
        WrappedSymbol = wrappedSymbol;
        WrappedTypeName = wrappedTypeName;
        TypeArguments = typeArguments;
        
        var builder = new StringBuilder(wrappedSymbol.Name);
        if (typeArguments.Length is not 0)
        {
            builder.AppendTypeParameters(typeArguments);
        }

        DiscriminatorTypeName = builder.ToString();
    }

    public TSymbol Symbol { get; }
    public string FieldName { get; }
    public Compilation Compilation { get; }
    public INamedTypeSymbol WrappedSymbol { get; }
    public string WrappedTypeName { get; }
    public string DiscriminatorTypeName { get; }
    public ImmutableArray<TypeArgument> TypeArguments { get; }

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
            return new MemberBuildingContext<T>(
                symbol, FieldName, Compilation, WrappedSymbol, WrappedTypeName, TypeArguments);
        }

        return null;
    }
}