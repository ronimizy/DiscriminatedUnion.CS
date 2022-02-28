using DiscriminatedUnion.CS.Generators.Pipeline.Models;

namespace DiscriminatedUnion.CS.Generators.Pipeline
{
    public interface IFileBuilder
    {
        FileBuildingContext BuildFile(FileBuildingContext context);
        IFileBuilder AddNext(IFileBuilder builder);
    }
}