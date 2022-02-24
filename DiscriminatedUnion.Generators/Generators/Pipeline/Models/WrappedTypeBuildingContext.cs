using DiscriminatedUnion.Generators.Generators.SourceComponents;
using DiscriminatedUnion.Generators.Generators.SourceComponents.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.Generators.Generators.Pipeline.Models
{
    public struct WrappedTypeBuildingContext
    {
        public WrappedTypeBuildingContext(INamedTypeSymbol symbol, TypeAlias alias, ISourceComponent component, string fieldName)
        {
            Symbol = symbol;
            Alias = alias;
            Component = component;
            FieldName = fieldName;
        }

        public INamedTypeSymbol Symbol { get; }
        public TypeAlias Alias { get; }
        public ISourceComponent Component { get; }
        public string FieldName { get; }
    }
}