using DiscriminatedUnion.CS.Generators.Pipeline;
using DiscriminatedUnion.CS.Generators.Pipeline.WrappedTypeBuilding;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DiscriminatedUnion.CS.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBuilders(this IServiceCollection collection)
        {
            using var scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker));

            scanner.EnqueueAdditionOfTypesThat()
                .WouldBeRegisteredAs<IMemberBuilder>()
                .WithSingletonLifetime()
                .MustBeAssignableTo<IMemberBuilder>()
                .AreNotInterfaces()
                .AreNotAbstractClasses();

            scanner.EnqueueAdditionOfTypesThat()
                .WouldBeRegisteredAs<IFileBuilder>()
                .WithSingletonLifetime()
                .MustBeAssignableTo<IFileBuilder>()
                .AreNotInterfaces()
                .AreNotAbstractClasses();

            scanner.EnqueueAdditionOfTypesThat()
                .WouldBeRegisteredAs<IWrappedTypeBuilder>()
                .WithSingletonLifetime()
                .MustBeAssignableTo<IWrappedTypeBuilder>()
                .AreNotInterfaces()
                .AreNotAbstractClasses();

            return collection;
        }
    }
}