using DiscriminatedUnion.CS.Annotations;

namespace DiscriminatedUnion.CS.Sample
{
    public class Success<T>
    {
        public Success(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public void A<V>() { }

        public static void B<V>() { }
    }

    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; init; }
    }

    [GeneratedDiscriminatedUnion]
    public abstract partial class B : IDiscriminator<int>, IDiscriminator<int, OtherInt32> { }

    [GeneratedDiscriminatedUnion]
    public abstract partial class Result<T> : IDiscriminator<Success<T>>, IDiscriminator<Error> { }

    [GeneratedDiscriminatedUnion]
    public abstract partial class GenericResult<T> : IDiscriminator<T, Success>, IDiscriminator<Error> { }
}