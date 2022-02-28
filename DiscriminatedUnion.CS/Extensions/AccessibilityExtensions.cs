using Microsoft.CodeAnalysis;

namespace DiscriminatedUnion.CS.Extensions
{
    public static class AccessibilityExtensions
    {
        public static string ToFormattedString(this Accessibility accessibility)
            => accessibility switch
            {
                Accessibility.Public => "public",
                Accessibility.Private => "private",
                Accessibility.Protected => "protected",
                Accessibility.Internal => "internal",
                _ => string.Empty,
            };
    }
}