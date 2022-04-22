namespace DiscriminatedUnion.CS.Utility;

public interface IResponsibilityChainLink<TLink> where TLink : IResponsibilityChainLink<TLink>
{
    TLink AddNext(TLink link);
}