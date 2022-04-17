using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace DiscriminatedUnion.CS.Extensions;

public static class RefKindExtensions
{
    public static SyntaxToken ToSyntaxToken(this RefKind refKind) => refKind switch
    {
        RefKind.None => Token(SyntaxKind.WhitespaceTrivia),
        RefKind.Ref => Token(SyntaxKind.RefKeyword),
        RefKind.Out => Token(SyntaxKind.OutKeyword),
        RefKind.In => Token(SyntaxKind.InKeyword),
        _ => throw new ArgumentOutOfRangeException(nameof(refKind), refKind, null)
    };
}