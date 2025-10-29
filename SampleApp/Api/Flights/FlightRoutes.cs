using Framework;
using Microsoft.AspNetCore.Mvc;
using SampleApp.Application.Commands;
using SampleApp.Application.Projections;
using SampleApp.Application.Queries;
using SampleApp.Domain.Flights;

namespace SampleApp.Api.Flights;

public static class FlightRoutes
{
    public static IEndpointRouteBuilder MapFlightRoutes(this IEndpointRouteBuilder app)
    {
        app.MapGet("/flights",
                ([FromServices] IQueryHandler<GetAllFlights, IAsyncEnumerable<FlightListItem>> handler) =>
                handler.Query(new GetAllFlights()))
            .WithSummary("Get all flights")
            .WithTags("Flights");
        
        app.MapPost("/flights",
                async ([FromBody] ScheduleFlight command, [FromServices] ICommandHandler<ScheduleFlight> handler) =>
                {
                    await handler.Handle(command);
                    return Results.Created();
                }).WithSummary("Schedule a new flight")
            .WithTags("Flights");

        app.MapPatch("/flights/{flightId}", async ([FromBody] UpdateFlight update,
                [FromRoute] Id<Flight> flightId,
                [FromServices] ICommandHandler<AssignAircraftToFlight> assign,
                [FromServices] ICommandHandler<DelayDeparture> delay) =>
            {
                if (update.AircraftId.HasValue)
                {
                    await assign.Handle(new AssignAircraftToFlight(flightId, update.AircraftId.Value));
                }

                if (update.DepartureTime.HasValue)
                {
                    await delay.Handle(new DelayDeparture(flightId, update.DepartureTime.Value));
                }
                
                return Results.NoContent();
            }).WithSummary("Update flight")
            .WithTags("Flights");

        app.MapPost("/flight/{flightId}/departure", async ([FromRoute] Id<Flight> flightId,
                [FromServices] ICommandHandler<Depart> depart) =>
            {
                await depart.Handle(new Depart(flightId));
                return Results.Created();
            }).WithSummary("Register flight departure")
            .WithTags("Flights");

        app.MapPost("/flight/{flightId}/arrival", async ([FromRoute] Id<Flight> flightId,
                [FromServices] ICommandHandler<Arrive> depart) =>
            {
                await depart.Handle(new Arrive(flightId));
                return Results.Created();
            }).WithSummary("Register flight arrival")
            .WithTags("Flights");
        
        return app;
    }
}