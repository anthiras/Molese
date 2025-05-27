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

    public static IServiceCollection RegisterEventSourcedRepositories(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var aggregateRoots = assemblies.SelectMany(assembly => assembly.GetTypes().Where(IsAggregateRoot));

        foreach (var aggregateRoot in aggregateRoots)
        {
            services.AddSingleton(
                typeof(IRepository<>).MakeGenericType(aggregateRoot),
                typeof(EventSourcedRepository<>).MakeGenericType(aggregateRoot));
        }

        return services;
    }
    
    public static IServiceCollection RegisterDocumentRepositories(this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var documents = assemblies.SelectMany(assembly => assembly.GetTypes().Where(IsDocument));

        foreach (var document in documents)
        {
            services.AddSingleton(
                typeof(IRepository<>).MakeGenericType(document),
                typeof(DocumentRepository<>).MakeGenericType(document));
        }

        return services;
    }

    private static bool IsAggregateRoot(Type type)
    {
        return type.BaseType is { IsGenericType: true } && type.BaseType.GetGenericTypeDefinition() == typeof(AggregateRoot<>);
    }
    
    private static bool IsDocument(Type type)
    {
        return type.BaseType is { IsGenericType: true } && type.BaseType.GetGenericTypeDefinition() == typeof(Document<>);
    }
}
