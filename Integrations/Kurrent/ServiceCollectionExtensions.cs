using EventStore.Client;
using Framework;
using Microsoft.Extensions.DependencyInjection;

namespace Kurrent;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKurrentEventStore(this IServiceCollection services, 
        EventStoreClientSettings settings)
    {
        return services
            .AddSingleton(new EventStoreClient(settings))
            .AddSingleton<IEventStore, KurrentEventStore>();
    }
}