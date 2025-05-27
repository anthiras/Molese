using Framework;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;
using Test;

namespace SampleApp.Test.Domain.Flights;

public class FlightTests
{
    [Theory, DomainAutoData]
    public void FlightCanBeConstructedFromEvents(Id<Flight> id, FlightScheduled e1, FlightAircraftAssigned e2)
    {
        var flight = new Flight(id, [e1, e2]);
        
        Assert.Equal(e1.Route, flight.Route);
        Assert.Equal(e1.Schedule, flight.OriginalSchedule);
        Assert.Equal(e2.AircraftId, flight.AircraftId);
    }

    [Theory, DomainAutoData]
    public void NewFlightIsScheduledWithGivenRouteAndSchedule(Id<Flight> flightId, Route route, Schedule schedule)
    {
        var flight = new Flight(flightId, route, schedule);

        Assert.Equal(route, flight.Route);
        Assert.Equal(schedule, flight.OriginalSchedule);
        AssertEvents.Raised(new FlightScheduled(flight.Id, route, schedule), flight);
    }

    [Theory, DomainAutoData]
    public void FlightIsDelayedWhenGivenADepartureTimeLaterThanOriginal(Flight flight)
    {
        var newDeparture = flight.OriginalSchedule!.DepartureTime.AddMinutes(40);
        flight.DelayDeparture(newDeparture);
        
        var expectedNewArrival = flight.OriginalSchedule!.ArrivalTime.AddMinutes(40);
        var expectedNewSchedule = new Schedule(newDeparture, expectedNewArrival);

        Assert.Equal(expectedNewSchedule, flight.NewSchedule);
        AssertEvents.Raised(new FlightDelayed(flight.Id, expectedNewSchedule), flight);
    }

    [Theory, DomainAutoData]
    public void FlightDelayWithAnEarlyDepartureTimeIsInvalid(Flight flight)
    {
        var newDeparture = flight.OriginalSchedule!.DepartureTime.AddMinutes(-10);

        Assert.Throws<ArgumentOutOfRangeException>(() => flight.DelayDeparture(newDeparture));
    }

    [Theory, DomainAutoData]
    public void FlightCannotDepartWithoutAnAssignedAircraft(Flight flight)
    {
        Assert.ThrowsAny<Exception>(() => flight.Depart(flight.OriginalSchedule!.DepartureTime));
    }

    [Theory, DomainAutoData]
    public void FlightCanBeAssignedToAnAircraft(Flight flight, Id<Aircraft> aircraftId)
    {
        flight.Assign(aircraftId);
        
        Assert.Equal(aircraftId, flight.AircraftId);
        AssertEvents.Raised(new FlightAircraftAssigned(flight.Id, aircraftId), flight);
    }

    [Theory, DomainAutoData]
    public void FlightCanDepartWithAircraft([Assigned] Flight flight)
    {
        flight.Depart(flight.OriginalSchedule!.DepartureTime);
        
        AssertEvents.Raised(new FlightDeparted(flight.Id, flight.OriginalSchedule!.DepartureTime), flight);
    }

    [Theory, DomainAutoData]
    public void FlightIsDelayedIfDepartedLaterThanPlanned([Assigned] Flight flight)
    {
        var delay = TimeSpan.FromMinutes(10);
        flight.Depart(flight.OriginalSchedule!.DepartureTime.Add(delay));

        AssertEvents.Raised(new FlightDelayed(flight.Id, flight.OriginalSchedule.Add(delay)), flight);
        AssertEvents.Raised(new FlightDeparted(flight.Id, flight.OriginalSchedule!.DepartureTime.Add(delay)), flight);
    }

    [Theory, DomainAutoData]
    public void FlightCannotArriveIfNotDeparted(Flight flight)
    {
        Assert.ThrowsAny<Exception>(() => flight.Arrive(flight.OriginalSchedule!.ArrivalTime));
    }

    [Theory, DomainAutoData]
    public void FlightCannotArriveEarlierThanDeparted([Departed] Flight flight)
    {
        Assert.ThrowsAny<Exception>(() => flight.Arrive(flight.OriginalSchedule!.DepartureTime.AddMinutes(-10)));
    }

    [Theory, DomainAutoData]
    public void FlightCanArriveAfterDeparture([Departed] Flight flight)
    {
        var arrivalTime = flight.OriginalSchedule!.DepartureTime.AddHours(1);
        
        flight.Arrive(arrivalTime);
        
        AssertEvents.Raised(new FlightArrived(flight.Id, arrivalTime), flight);
    }
}