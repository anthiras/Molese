using SampleApp.Domain.Flights;
using SampleApp.Domain.Time;

namespace SampleApp.Domain.Aircrafts;

public record Assignment(Id<Flight> FlightId, TimeRange TimeRange);