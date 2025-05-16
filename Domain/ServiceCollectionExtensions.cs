using Domain.Aircrafts;
using Domain.Flights;
using Microsoft.Extensions.DependencyInjection;

namespace Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        return services
            .AddEventSourcedAggregate<Aircraft>(Aircraft.Create)
            .AddEventSourcedAggregate<Flight>(Flight.Create);
    }

    private static IServiceCollection AddEventSourcedAggregate<T>(this IServiceCollection services,
        Func<Id<T>, IEnumerable<Event<T>>, T> factory) where T : AggregateRoot<T>
    {
        return services.AddSingleton<IRepository<T, Id<T>>>(sp =>
            ActivatorUtilities.CreateInstance<EventSourcedRepository<T>>(sp, factory));
    }
}