using Framework;
using Microsoft.Extensions.DependencyInjection;

namespace InMemory;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryEventStore(this IServiceCollection services)
    {
        return services.AddSingleton<IEventStore, InMemoryEventStore>();
    }

    public static IServiceCollection AddInMemoryDocumentStore(this IServiceCollection services)
    {
        return services.AddSingleton<InMemoryDocumentStore>()
            .AddSingleton<IDocumentStore>(provider => provider.GetRequiredService<InMemoryDocumentStore>())
            .AddSingleton<IDocumentStore>(provider => provider.GetRequiredService<InMemoryDocumentStore>());
    }
}