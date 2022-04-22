using DiscriminatedUnion.CS.Utility;

namespace DiscriminatedUnion.CS.Extensions;

public static class ResponsibilityChainLinkExtensions
{
    public static TLink Aggregate<TLink>(this IEnumerable<TLink> chain) where TLink : IResponsibilityChainLink<TLink>
        => chain.Aggregate((a, b) => a.AddNext(b));
}