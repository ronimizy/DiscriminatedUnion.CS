using DiscriminatedUnion.CS.Generators.SourceComponents;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators.Pipeline.Models
{
    public struct FileBuildingContext
    {
        public FileBuildingContext(ClassDeclarationSyntax syntax, ITypeSymbol symbol, ISourceComponent component)
        {
            Syntax = syntax;
            Symbol = symbol;
            Component = component;
        }

        public ClassDeclarationSyntax Syntax { get; }
        public ITypeSymbol Symbol { get; }
        public ISourceComponent Component { get; set; }

        public FileBuildingContext WithComponent(ISourceComponent component)
            => new FileBuildingContext(Syntax, Symbol, component);

        public void Deconstruct(
            out ClassDeclarationSyntax syntax, out ITypeSymbol symbol, out ISourceComponent component)
        {
            (syntax, symbol, component) = (Syntax, Symbol, Component);
        }
    }
}