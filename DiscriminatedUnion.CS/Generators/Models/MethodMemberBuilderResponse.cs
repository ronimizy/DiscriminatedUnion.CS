using System;

namespace DiscriminatedUnion.CS.Generators.Models
{
    [Flags]
    public enum MethodMemberBuilderResponse
    {
        Built = 1,
        NotBuilt = 2,
        Invalid = 4,
    }
}