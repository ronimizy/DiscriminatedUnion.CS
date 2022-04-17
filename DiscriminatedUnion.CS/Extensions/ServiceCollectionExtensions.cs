using DiscriminatedUnion.CS.Generators.Pipeline;
using FluentScanning;
using FluentScanning.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DiscriminatedUnion.CS.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBuilders(this IServiceCollection collection)
    {
        using var scanner = collection.UseAssemblyScanner(typeof(IAssemblyMarker));

        scanner.EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAs<ICompilationUnitBuilder>()
            .WithSingletonLifetime()
            .MustBeAssignableTo<ICompilationUnitBuilder>()
            .AreNotInterfaces()
            .AreNotAbstractClasses();

        scanner.EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAs<IDiscriminatorBuilder>()
            .WithSingletonLifetime()
            .MustBeAssignableTo<IDiscriminatorBuilder>()
            .AreNotInterfaces()
            .AreNotAbstractClasses();

        scanner.EnqueueAdditionOfTypesThat()
            .WouldBeRegisteredAs<IUnionBuilder>()
            .WithSingletonLifetime()
            .MustBeAssignableTo<IUnionBuilder>()
            .AreNotInterfaces()
            .AreNotAbstractClasses();

        return collection;
    }
}