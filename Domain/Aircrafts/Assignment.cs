using Domain.Flights;
using Domain.Time;

namespace Domain.Aircrafts;

public record Assignment(Id<Flight> FlightId, TimeRange TimeRange);