using Framework;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;

namespace SampleApp.Application.Commands;

public record ScheduleFlight(Route Route, Schedule Schedule);

public record AssignAircraftToFlight(Id<Flight> FlightId, Id<Aircraft> AircraftId);

public record DelayDeparture(Id<Flight> FlightId, DateTime NewDepartureTime);

public record Depart(Id<Flight> FlightId);

public record Arrive(Id<Flight> FlightId);