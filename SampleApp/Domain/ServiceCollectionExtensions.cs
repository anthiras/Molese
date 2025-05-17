using Microsoft.Extensions.DependencyInjection;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;

namespace SampleApp.Domain;

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
        return services.AddSingleton<IRepository<T>>(sp =>
            ActivatorUtilities.CreateInstance<EventSourcedRepository<T>>(sp, factory));
    }
}