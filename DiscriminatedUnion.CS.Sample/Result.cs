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
    }

    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    [GeneratedDiscriminatedUnion]
    public abstract partial class Result<T> : IDiscriminator<Success<T>>, IDiscriminator<Error> { }
}