using DiscriminatedUnion.CS.Generators.Pipeline.Models;

namespace DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding
{
    public abstract class WrappedTypeBuilderBase : IWrappedTypeBuilder
    {
        private IWrappedTypeBuilder? _next;
        
        public void BuildWrappedType(WrappedTypeBuildingContext context)
        {
            BuildWrappedTypePrivate(context);
            _next?.BuildWrappedType(context);
        }

        public IWrappedTypeBuilder AddNext(IWrappedTypeBuilder next)
        {
            if (_next is null)
                _next = next;
            else
                _next.AddNext(next);

            return this;
        }

        protected abstract void BuildWrappedTypePrivate(WrappedTypeBuildingContext context);
    }
}