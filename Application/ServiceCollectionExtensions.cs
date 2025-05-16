using System.Windows.Input;
using Application.Commands;
using Application.Projections;
using Application.Queries;
using Domain.Aircrafts;
using Framework;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services.AddProjections()
            .AddCommands()
            .AddQueries();
    }

    private static IServiceCollection AddProjections(this IServiceCollection services)
    {
        return services.AddSingleton<ProjectionSubscriber>()
            .AddSingleton<IEventHandler, AircraftListView>();
    }

    private static IServiceCollection AddCommands(this IServiceCollection services)
    {
        return services.AddSingleton<ICommandHandler<CreateAircraft>, AircraftCommandHandler>()
            .AddSingleton<ICommandHandler<DeleteAircraft>, AircraftCommandHandler>();
    }

    private static IServiceCollection AddQueries(this IServiceCollection services)
    {
        return services.AddSingleton<IQueryHandler<GetAllAircrafts, IAsyncEnumerable<AircraftListItem>>, AircraftQueryHandler>()
            .AddSingleton<IQueryHandler<GetAircraftById, Task<AircraftListItem>>, AircraftQueryHandler>();
    }
}