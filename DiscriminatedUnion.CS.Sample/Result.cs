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
    public abstract partial class Result
    {
        public partial class Success<T> : IDiscriminator<Sample.Success<T>> { }
        public partial class Error : IDiscriminator<Sample.Error> { }
    }
}