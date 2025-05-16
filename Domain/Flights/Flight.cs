using Domain.Aircrafts;

namespace Domain.Flights;

public partial class Flight : AggregateRoot<Flight>
{
    /// <summary>
    /// Recreate from events
    /// </summary>
    /// <param name="id"></param>
    /// <param name="events"></param>
    public Flight(Id<Flight> id, IEnumerable<Event<Flight>> events) : base(id, events)
    {
    }

    /// <summary>
    /// Create new
    /// </summary>
    /// <param name="id"></param>
    /// <param name="route"></param>
    /// <param name="schedule"></param>
    public Flight(Id<Flight> id, Route route, Schedule schedule) : base(id)
    {
        Raise(new FlightScheduled(Id, route, schedule));
    }

    public static Flight Create(Id<Flight> id, IEnumerable<Event<Flight>> events)
    {
        return new Flight(id, events);
    }
    
    public Route? Route { get; private set; }
    public Schedule? Schedule { get; private set; }
    public Schedule? NewSchedule { get; private set; }
    public Id<Aircraft>? AircraftId { get; private set; }
    public DateTime? ActualDepartureTime { get; private set; }
    public DateTime? ActualArrivalTime { get; private set; }

    public void Assign(Id<Aircraft> aircraftId)
    {
        Raise(new FlightAircraftAssigned(Id, aircraftId));
    }

    public void DelayDeparture(DateTime newDeparture)
    {
        if (newDeparture <= Schedule!.DepartureTime)
            throw new ArgumentOutOfRangeException(nameof(newDeparture), newDeparture,
                "New departure time is not later than the scheduled departure time");
        
        var newArrival = Schedule!.ArrivalTime.Add(newDeparture - Schedule!.DepartureTime);
        
        Raise(new FlightDelayed(Id, new Schedule(newDeparture, newArrival)));
    }
    
    public void Depart(DateTime departureTime)
    {
        if (!AircraftId.HasValue)
            throw new InvalidOperationException("No aircraft assigned");
        
        if (NewSchedule != null && departureTime != NewSchedule?.DepartureTime)
            DelayDeparture(departureTime);
        else if (departureTime != Schedule!.DepartureTime)
            DelayDeparture(departureTime);
        
        Raise(new FlightDeparted(Id, departureTime));
    }

    public void Arrive(DateTime arrivalTime)
    {
        if (!ActualDepartureTime.HasValue)
            throw new InvalidOperationException("Flight is not departed");
        
        if (arrivalTime < ActualDepartureTime)
            throw new ArgumentOutOfRangeException(nameof(arrivalTime), arrivalTime,
                "Arrival time cannot be earlier than departure time");
        
        Raise(new FlightArrived(Id, arrivalTime));
    }

    private void Apply(FlightScheduled @event)
    {
        Route = @event.Route;
        Schedule = @event.Schedule;
    }

    private void Apply(FlightDelayed @event)
    {
        NewSchedule = @event.NewSchedule;
    }

    private void Apply(FlightAircraftAssigned @event)
    {
        AircraftId = @event.AircraftId;
    }

    private void Apply(FlightDeparted @event)
    {
        ActualDepartureTime = @event.DepartureTime;
    }
    
    private void Apply(FlightArrived @event)
    {
        ActualArrivalTime = @event.ArrivalTime;
    }

}