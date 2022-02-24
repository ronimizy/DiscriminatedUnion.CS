using DiscriminatedUnion.Generators.Generators.Pipeline.Models;

namespace DiscriminatedUnion.Generators.Generators.Pipeline
{
    public interface IWrappedTypeBuilder
    {
        void BuildWrappedType(WrappedTypeBuildingContext context);
        IWrappedTypeBuilder AddNext(IWrappedTypeBuilder next);
    }
}