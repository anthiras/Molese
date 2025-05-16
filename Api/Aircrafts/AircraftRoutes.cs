using Application.Commands;
using Domain.Aircrafts;
using Framework;

namespace Api.Aircrafts;

public static class AircraftRoutes
{
    public static IEndpointRouteBuilder MapAircraftRoutes(this IEndpointRouteBuilder app)
    {
        app.MapPost("/aircrafts", async (CreateAircraft command, ICommandHandler<CreateAircraft> handler) =>
        {
            await handler.Handle(command);
            return Results.Created();
        }).WithSummary("Create a new aircraft")
        .WithTags("Aircrafts");

        app.MapDelete("/aircrafts/{id}", async (Id<Aircraft> id, ICommandHandler<DeleteAircraft> handler) =>
        {
            await handler.Handle(new DeleteAircraft(id));
            return Results.NoContent();
        }).WithSummary("Delete an aircraft")
        .WithTags("Aircrafts");

        return app;
    }
}