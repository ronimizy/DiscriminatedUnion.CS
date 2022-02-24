using DiscriminatedUnion.Generators.Generators.Pipeline.Models;

namespace DiscriminatedUnion.Generators.Generators.Pipeline
{
    public interface IFileBuilder
    {
        FileBuildingContext BuildFile(FileBuildingContext context);
        IFileBuilder AddNext(IFileBuilder builder);
    }
}