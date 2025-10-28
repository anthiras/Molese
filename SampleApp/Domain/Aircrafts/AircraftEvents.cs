using SampleApp.Domain.Flights;
using SampleApp.Domain.Time;

namespace SampleApp.Domain.Aircrafts;

public record AircraftCreated(Id<Aircraft> AircraftId, AircraftRegistration Registration) : Event<Aircraft>(AircraftId);

public record AircraftDeleted(Id<Aircraft> AircraftId) : Event<Aircraft>(AircraftId);

public record AircraftAssignedToFlight(Id<Aircraft> AircraftId, Id<Flight> FlightId, TimeRange TimeRange) : Event<Aircraft>(AircraftId);

public record AircraftUnassignedFromFlight(Id<Aircraft> AircraftId, Id<Flight> FlightId, TimeRange TimeRange) : Event<Aircraft>(AircraftId);