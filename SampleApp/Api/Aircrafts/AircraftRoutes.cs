using SampleApp.Application.Commands;
using SampleApp.Application.Projections;
using SampleApp.Application.Queries;
using SampleApp.Domain.Aircrafts;
using Framework;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Api.Aircrafts;

public static class AircraftRoutes
{
    public static IEndpointRouteBuilder MapAircraftRoutes(this IEndpointRouteBuilder app)
    {
        app.MapPost("/aircrafts", async ([FromBody] CreateAircraft command, [FromServices] ICommandHandler<CreateAircraft> handler) =>
        {
            await handler.Handle(command);
            return Results.Created();
        }).WithSummary("Create a new aircraft")
        .WithTags("Aircrafts");
        
        app.MapDelete("/aircrafts/{id}", async (Id<Aircraft> id, [FromServices] ICommandHandler<DeleteAircraft> handler) =>
        {
            await handler.Handle(new DeleteAircraft(id));
            return Results.NoContent();
        }).WithSummary("Delete an aircraft")
        .WithTags("Aircrafts");

        app.MapGet("/aircrafts",
                ([FromServices] IQueryHandler<GetAllAircrafts, IAsyncEnumerable<AircraftListItem>> queryHandler) =>
                    queryHandler.Query(new GetAllAircrafts()))
            .WithSummary("Get all aircrafts")
            .WithTags("Aircrafts");

        app.MapGet("/aircrafts/{id}",
                (Id<Aircraft> id, [FromServices] IQueryHandler<GetAircraftById, Task<AircraftListItem>> queryHandler) =>
                    queryHandler.Query(new GetAircraftById(id)))
            .WithSummary("Get aircraft by ID")
            .WithTags("Aircrafts");

        return app;
    }
}