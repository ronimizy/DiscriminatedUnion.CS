using System;

namespace DiscriminatedUnion.Generators.Generators.Models
{
    [Flags]
    public enum MethodMemberBuilderResponse
    {
        Built = 1,
        NotBuilt = 2,
        Invalid = 4,
    }
}