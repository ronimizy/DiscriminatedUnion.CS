namespace DiscriminatedUnion.CS.Utility;

public static class EqualityComparerFactory
{
    public static IEqualityComparer<T> Create<T>(Func<T, T, bool> comparison)
        => new GenericComparer<T>(comparison);

    private class GenericComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _comparison;

        public GenericComparer(Func<T, T, bool> comparison)
        {
            _comparison = comparison;
        }

        public bool Equals(T x, T y)
            => _comparison.Invoke(x, y);

        public int GetHashCode(T obj)
            => obj?.GetHashCode() ?? 0;
    }
}