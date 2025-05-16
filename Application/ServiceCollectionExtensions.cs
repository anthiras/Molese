using Application.Projections;
using Framework;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services.AddSingleton<ProjectionSubscriber>()
            .AddSingleton<IEventHandler, AircraftListView>();
    }
}