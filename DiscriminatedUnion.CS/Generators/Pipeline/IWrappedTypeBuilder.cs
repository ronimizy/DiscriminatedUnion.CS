using DiscriminatedUnion.CS.Generators.Pipeline.Models;

namespace DiscriminatedUnion.CS.Generators.Pipeline
{
    public interface IWrappedTypeBuilder
    {
        void BuildWrappedType(WrappedTypeBuildingContext context);
        IWrappedTypeBuilder AddNext(IWrappedTypeBuilder next);
    }
}