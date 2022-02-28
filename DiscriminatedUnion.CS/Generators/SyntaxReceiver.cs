using System.Collections.Generic;
using System.Linq;
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

        var baseList = classNode.BaseList;
            
        if (baseList is null)
            return;

        var types = baseList
            .DescendantNodes()
            .OfType<GenericNameSyntax>()
            .Select(s => s.Identifier);

        if (!types.Any(i => i.ToString().Equals(Definer.UnionWithInterfaceName)))
            return;

        _nodes.Add(classNode);
    }
}