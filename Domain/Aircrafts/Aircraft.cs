using Domain.Time;

namespace Domain.Aircrafts;

public partial class Aircraft : AggregateRoot<Aircraft>
{
    public Aircraft(Id<Aircraft> id, IEnumerable<Event<Aircraft>> events) : base(id, events)
    {
    }

    public Aircraft(Id<Aircraft> id, AircraftRegistration registration) : base(id)
    {
        Raise(new AircraftCreated(Id, registration));
    }

    public static Aircraft Create(Id<Aircraft> id, IEnumerable<Event<Aircraft>> events)
    {
        return new Aircraft(id, events);
    }

    public AircraftRegistration? Registration { get; private set; }
    
    private readonly List<Assignment> _assignments = [];
    
    public IReadOnlyList<Assignment> Assignments => _assignments;

    public bool IsAvailable(TimeRange timeRange)
    {
        return !_assignments.Any(assignment => timeRange.Overlaps(assignment.TimeRange));
    }

    public void Assign(Assignment assignment)
    {
        if (!IsAvailable(assignment.TimeRange))
            throw new ArgumentOutOfRangeException(nameof(assignment.TimeRange), assignment.TimeRange, "Aircraft is unavailable");
        
        Raise(new AircraftAssignedToFlight(Id, assignment.FlightId, assignment.TimeRange));
    }
    
    public void Unassign(Assignment assignment)
    {
        if (!_assignments.Contains(assignment))
            throw new ArgumentException("Assignment not found", nameof(assignment));
        
        Raise(new AircraftUnassignedFromFlight(Id, assignment.FlightId, assignment.TimeRange));
    }
    
    private void Apply(AircraftCreated @event)
    {
        Registration = @event.Registration;
    }

    private void Apply(AircraftAssignedToFlight @event)
    {
        _assignments.Add(new Assignment(@event.FlightId, @event.TimeRange));
    }

    private void Apply(AircraftUnassignedFromFlight @event)
    {
        _assignments.Remove(new Assignment(@event.FlightId, @event.TimeRange));
    }
}