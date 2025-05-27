using Framework;
using InMemory;
using SampleApp.Application.Commands;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;
using SampleApp.Domain.Time;
using SampleApp.Test.Domain;
using SampleApp.Test.Domain.Aircrafts;
using Test;

namespace SampleApp.Test.Application.Commands;

public class FlightCommandTests
{
    private readonly FlightCommandHandler _sut;
    private readonly IRepository<Flight> _flightRepo = new InMemoryRepository<Flight>();
    private readonly IRepository<Aircraft> _aircraftRepo = new InMemoryRepository<Aircraft>();

    public FlightCommandTests()
    {
        _sut = new FlightCommandHandler(_flightRepo, _aircraftRepo);
    }

    [Theory, DomainAutoData]
    public async Task FailsToAssignUnavailableAircraft([Assigned] Aircraft aircraft, Route route)
    {
        // Arrange
        await _aircraftRepo.Store(aircraft);
        var schedule = new Schedule(aircraft.Assignments[0].TimeRange.StartTime, aircraft.Assignments[0].TimeRange.EndTime);
        var flight = new Flight(Id<Flight>.New(), route, schedule);
        await _flightRepo.Store(flight);

        // Act
        await Assert.ThrowsAnyAsync<Exception>(() => _sut.Handle(new AssignAircraftToFlight(flight.Id, aircraft.Id)));
        
        // Assert
        Assert.Null((await _flightRepo.Find(flight.Id)).AircraftId);
    }

    [Theory, DomainAutoData]
    public async Task AssignsAircraftToFlightWhenAvailable(Aircraft aircraft, Flight flight)
    {
        await _aircraftRepo.Store(aircraft);
        await _flightRepo.Store(flight);
        
        await _sut.Handle(new AssignAircraftToFlight(flight.Id, aircraft.Id));
        
        Assert.Equal(aircraft.Id, flight.AircraftId);
        Assert.Contains(new Assignment(flight.Id, flight.CurrentSchedule), aircraft.Assignments);
    }
}