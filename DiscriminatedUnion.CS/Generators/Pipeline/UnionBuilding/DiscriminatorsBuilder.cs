using DiscriminatedUnion.CS.Extensions;
using DiscriminatedUnion.CS.Generators.Pipeline.Models;
using DiscriminatedUnion.CS.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Generators.Pipeline.UnionBuilding;

public class DiscriminatorsBuilder : BuilderBase
{
    private const string FieldName = "_value";
    private static readonly IdentifierNameSyntax FieldNameIdentifier = IdentifierName(FieldName);

    private readonly IDiscriminatorBuilder _discriminatorBuilder;

    public DiscriminatorsBuilder(IEnumerable<IDiscriminatorBuilder> discriminatorBuilder)
    {
        _discriminatorBuilder = discriminatorBuilder.Aggregate();
    }

    protected override TypeDeclarationSyntax BuildDiscriminatorTypeDeclarationSyntaxProtected(
        UnionBuildingContext context)
    {
        (var unionSyntax, var unionType, IReadOnlyCollection<Discriminator>? discriminators) = context;

        foreach (var discriminator in discriminators)
        {
            var discriminatorTypeSyntax = GenerateDiscriminator(unionType, discriminator);
            unionSyntax = unionSyntax.AddMembers(discriminatorTypeSyntax);
        }

        return unionSyntax;
    }

    private TypeDeclarationSyntax GenerateDiscriminator(UnionType unionType, Discriminator discriminator)
    {
        TypeDeclarationSyntax typeSyntax = ClassDeclaration(discriminator.Name.Identifier);

        var wrappedContext = new DiscriminatorTypeBuildingContext(
            typeSyntax,
            unionType,
            discriminator,
            FieldNameIdentifier);

        return _discriminatorBuilder.BuildDiscriminatorTypeSyntax(wrappedContext);
    }
}