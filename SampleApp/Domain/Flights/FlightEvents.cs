using SampleApp.Domain.Time;
using SampleApp.Domain.Aircrafts;

namespace SampleApp.Domain.Flights;

public record FlightScheduled(Id<Flight> FlightId, Route Route, Schedule Schedule) : Event<Flight>(FlightId);

public record FlightDeparted(Id<Flight> FlightId, DateTime DepartureTime) : Event<Flight>(FlightId);

public record FlightArrived(Id<Flight> FlightId, DateTime ArrivalTime) : Event<Flight>(FlightId);

public record FlightDelayed(Id<Flight> FlightId, Schedule NewSchedule) : Event<Flight>(FlightId);

public record FlightAircraftAssigned(Id<Flight> FlightId, Id<Aircraft> AircraftId) : Event<Flight>(FlightId);