using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Framework;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEventSubscriber(this IServiceCollection services)
    {
        return services.AddSingleton<EventSubscriber>();
    }

    public static IServiceCollection RegisterHandlersFromAssemblies(this IServiceCollection services, params Assembly[] assemblies)
    {
        return services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AddClasses(classes => classes.AssignableTo<IEventHandler>())
            .AsImplementedInterfaces()
            .WithSingletonLifetime());
    }
    
    public static IServiceCollection RegisterAggregateRootRepository<T>(this IServiceCollection services,
        Func<Id<T>, IEnumerable<Event<T>>, T> factory) where T : AggregateRoot<T>
    {
        return services.AddSingleton<IRepository<T>>(sp =>
            ActivatorUtilities.CreateInstance<EventSourcedRepository<T>>(sp, factory));
    }

    public static IServiceCollection RegisterDocumentRepository<T>(this IServiceCollection services) where T : Document<T>
    {
        return services.AddSingleton<IRepository<T>, DocumentRepository<T>>();
    }
}
