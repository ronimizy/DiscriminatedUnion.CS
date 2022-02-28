using DiscriminatedUnion.CS.Annotations;

namespace DiscriminatedUnion.CS.Sample
{
    public class Success
    {
        public Success(double value)
        {
            Value = value;
        }

        public double Value { get; }
    }

    public class Error
    {
        public Error(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }

    public abstract partial class Result : IUnionWith<Success>, IUnionWith<Error> { }
}