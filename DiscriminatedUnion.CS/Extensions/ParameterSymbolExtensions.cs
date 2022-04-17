using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

public static class ParameterSymbolExtensions
{
    public static IEnumerable<ParameterSyntax> ToParameterSyntax(this IEnumerable<IParameterSymbol> parameterSymbols) 
        => parameterSymbols.Select(ToParameterSyntax);

    public static IEnumerable<ArgumentSyntax> ToArgumentSyntax(this IEnumerable<IParameterSymbol> parameterSymbols)
        => parameterSymbols.Select(ToArgumentSyntax);

    public static ParameterSyntax ToParameterSyntax(this IParameterSymbol symbol)
    {
        TypeSyntax type = symbol.Type.NullableAnnotation switch
        {
            NullableAnnotation.Annotated => NullableType(IdentifierName(symbol.Type.GetFullyQualifiedName())),
            _ => IdentifierName(symbol.Type.GetFullyQualifiedName()),
        };
        
        var parameter = Parameter(Identifier(symbol.Name))
            .WithType(type);

        if (symbol.RefKind is not RefKind.None)
        {
            parameter = parameter.AddModifiers(symbol.RefKind.ToSyntaxToken());
        }

        return parameter;
    }
    
    public static ArgumentSyntax ToArgumentSyntax(this IParameterSymbol symbol)
    {
        var parameter = Argument(IdentifierName(symbol.Name));

        if (symbol.RefKind is not RefKind.None)
        {
            parameter = parameter.WithRefKindKeyword(symbol.RefKind.ToSyntaxToken());
        }

        return parameter;
    }
}