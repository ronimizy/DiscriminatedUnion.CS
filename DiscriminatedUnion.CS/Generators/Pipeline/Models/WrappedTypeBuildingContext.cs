using DiscriminatedUnion.CS.Generators.SourceComponents;
using DiscriminatedUnion.CS.Generators.SourceComponents.Models;
using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models
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