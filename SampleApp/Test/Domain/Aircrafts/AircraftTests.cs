using Framework;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;
using SampleApp.Domain.Time;
using Test;

namespace SampleApp.Test.Domain.Aircrafts;

public class AircraftTests
{
    [Theory, DomainAutoData]
    public void AircraftIsCreatedWithIdAndRegistration(Id<Aircraft> id, AircraftRegistration registration)
    {
        var aircraft = new Aircraft(id, registration);
        
        AssertEvents.Raised(new AircraftCreated(id, registration), aircraft);
    }

    [Theory, DomainAutoData]
    public void AircraftWithoutAssignmentsIsAvailableAtAnyTime(Aircraft aircraft, TimeRange timeRange)
    {
        Assert.True(aircraft.IsAvailable(timeRange));
    }

    [Theory, DomainAutoData]
    public void AircraftIsAssignedToFlightWhenAvailable(Aircraft aircraft, Assignment assignment)
    {
        aircraft.Assign(assignment);
        
        AssertEvents.Raised(new AircraftAssignedToFlight(aircraft.Id, assignment.FlightId, assignment.TimeRange), aircraft);
    }
    
    [Theory, DomainAutoData]
    public void AircraftIsUnavailableWhenAssignedDuringGivenTimeRange(Aircraft aircraft, Id<Flight> flightId)
    {
        var assignedTime = new TimeRange(DateTime.Today, DateTime.Today.AddHours(4));
        aircraft.Assign(new Assignment(flightId, assignedTime));
        
        Assert.False(aircraft.IsAvailable(assignedTime));
        Assert.False(aircraft.IsAvailable(new TimeRange(DateTime.Today.AddHours(-3), DateTime.Today.AddHours(5))));
        Assert.False(aircraft.IsAvailable(new TimeRange(DateTime.Today.AddHours(-3), DateTime.Today.AddHours(1))));
        Assert.False(aircraft.IsAvailable(new TimeRange(DateTime.Today.AddHours(2), DateTime.Today.AddHours(5))));
    }

    [Theory, DomainAutoData]
    public void AircraftFailsAssignmentDuringUnavailableTime([Assigned] Aircraft aircraft)
    {
        var assignment = aircraft.Assignments[0];

        Assert.ThrowsAny<Exception>(() => aircraft.Assign(assignment));
    }

    [Theory, DomainAutoData]
    public void AircraftIsUnassigned([Assigned] Aircraft aircraft)
    {
        var assignment = aircraft.Assignments[0];

        aircraft.Unassign(assignment);
        
        AssertEvents.Raised(new AircraftUnassignedFromFlight(aircraft.Id, assignment.FlightId, assignment.TimeRange), aircraft);
    }

    [Theory, DomainAutoData]
    public void AircraftFailsRemovingUnknownAssignment([Assigned] Aircraft aircraft, Assignment unknownAssignment)
    {
        Assert.ThrowsAny<Exception>(() => aircraft.Unassign(unknownAssignment));
    }
}