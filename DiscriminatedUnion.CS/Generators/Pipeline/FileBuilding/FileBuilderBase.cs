using DiscriminatedUnion.CS.Generators.Pipeline.Models;

namespace DiscriminatedUnion.CS.Generators.Pipeline.FileBuilding
{
    public abstract class FileBuilderBase : IFileBuilder
    {
        protected IFileBuilder? Next;

        public FileBuildingContext BuildFile(FileBuildingContext context)
        {
            context = BuildFileBase(context);
            return Next?.BuildFile(context) ?? context;
        }

        public IFileBuilder AddNext(IFileBuilder builder)
        {
            if (Next is null)
                Next = builder;
            else
                Next.AddNext(builder);

            return this;
        }

        protected abstract FileBuildingContext BuildFileBase(FileBuildingContext context);
    }
}