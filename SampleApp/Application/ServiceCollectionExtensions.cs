using System.Windows.Input;
using SampleApp.Domain.Aircrafts;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using SampleApp.Application.Commands;
using SampleApp.Application.Projections;
using SampleApp.Application.Queries;

namespace SampleApp.Application;

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
        return services.AddSingleton<IEventHandler, AircraftListView>();
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