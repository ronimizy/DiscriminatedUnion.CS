using DiscriminatedUnion.CS.Utility;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DiscriminatedUnion.CS.Generators;

public class SyntaxReceiver : ISyntaxReceiver
{
    private readonly List<ClassDeclarationSyntax> _nodes = new List<ClassDeclarationSyntax>();

    public IReadOnlyCollection<ClassDeclarationSyntax> Nodes => _nodes;

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        var classNode = syntaxNode as ClassDeclarationSyntax;

        if (classNode is null)
            return;

        var attributes = classNode.AttributeLists
            .SelectMany(a => a.Attributes);

        if (!attributes.Any(a => Definer.DiscriminatedUnionAttributeName.StartsWith(a.Name.ToString())))
            return;

        _nodes.Add(classNode);
    }
}