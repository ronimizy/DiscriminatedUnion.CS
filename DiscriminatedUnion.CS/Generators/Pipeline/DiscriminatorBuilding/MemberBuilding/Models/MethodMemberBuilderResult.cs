namespace DiscriminatedUnion.CS.Generators.Pipeline.DiscriminatorBuilding.MemberBuilding.Models;

[Flags]
public enum MethodMemberBuilderResult
{
    Built = 1,
    NotBuilt = 2,
    Invalid = 4,
}