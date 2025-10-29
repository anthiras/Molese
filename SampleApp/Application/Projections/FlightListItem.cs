using Framework;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;

namespace SampleApp.Application.Projections;

public class FlightListItem : Document<FlightListItem>
{
    public required Id<Flight> FlightId { get; init; }
    public static Id<FlightListItem> DocId(Id<Flight> id) => new(id.Value);
    public Route Route { get; set; }
    public Schedule OriginalSchedule { get; set; }
    public Schedule? NewSchedule { get; set; }
    public Id<Aircraft>? AircraftId { get; set; }
    public DateTime? ActualDepartureTime { get; set; }
    public DateTime? ActualArrivalTime { get; set; }
    public FlightStatus Status { get; set; }
}

public enum FlightStatus
{
    Scheduled,
    Delayed,
    Departed,
    Arrived
}