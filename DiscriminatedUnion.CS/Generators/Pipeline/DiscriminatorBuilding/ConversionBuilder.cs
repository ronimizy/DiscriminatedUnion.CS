using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding;

public class ConversionBuilder : DiscriminatorBuilderBase
{
    private const string ParameterName = "value";
    private static readonly SyntaxToken ParameterIdentifier = Identifier(ParameterName);
    private static readonly IdentifierNameSyntax ParameterIdentifierName = IdentifierName(ParameterName);
    private static readonly ArgumentSyntax ParameterArgument = Argument(ParameterIdentifierName);
    
    protected override TypeDeclarationSyntax BuildWrappedTypeDeclarationSyntaxProtected(DiscriminatorTypeBuildingContext context)
    {
        
        var (declaration, _, discriminator, fieldName) = context;
        
        var creation = ObjectCreationExpression(discriminator.Name)
            .AddArgumentListArguments(ParameterArgument);

        var memberAccess = MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, 
            ParameterIdentifierName, fieldName);

        var toDiscriminator = discriminator.WrappedTypeName
            .ToConversion(discriminator.Name, ParameterIdentifier)
            .AddBodyStatements(ReturnStatement(creation));

        var fromDiscriminator = discriminator.Name
            .ToConversion(discriminator.WrappedTypeName, ParameterIdentifier)
            .AddBodyStatements(ReturnStatement(memberAccess));
        
        return declaration.AddMembers(toDiscriminator, fromDiscriminator);
    }
}