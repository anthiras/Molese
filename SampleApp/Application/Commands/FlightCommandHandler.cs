using System.Data;
using Framework;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;

namespace SampleApp.Application.Commands;

public class FlightCommandHandler(IRepository<Flight> flightRepo, IRepository<Aircraft> aircraftRepo) : 
    ICommandHandler<ScheduleFlight>,
    ICommandHandler<AssignAircraftToFlight>,
    ICommandHandler<DelayDeparture>,
    ICommandHandler<Depart>,
    ICommandHandler<Arrive>
{
    public Task Handle(ScheduleFlight command)
    {
        var flight = new Flight(Id<Flight>.New(), command.Route, command.Schedule);

        return flightRepo.Store(flight);
    }

    public async Task Handle(AssignAircraftToFlight command)
    {
        var aircraft = await aircraftRepo.Find(command.AircraftId);
        var flight = await flightRepo.Find(command.FlightId);
        var assignment = new Assignment(flight.Id, flight.CurrentSchedule);
        
        aircraft.Assign(assignment);
        await aircraftRepo.Store(aircraft);

        try
        {
            flight.Assign(aircraft.Id);
            await flightRepo.Store(flight);
        }
        catch (DBConcurrencyException)
        {
            aircraft.Unassign(assignment);
            await aircraftRepo.Store(aircraft);
            throw;
        }
    }

    public Task Handle(DelayDeparture command)
    {
        return flightRepo.Update(command.FlightId, flight => flight.DelayDeparture(command.NewDepartureTime));
    }

    public Task Handle(Depart command)
    {
        return flightRepo.Update(command.FlightId, flight => flight.Depart(DateTime.Now));
    }

    public Task Handle(Arrive command)
    {
        return flightRepo.Update(command.FlightId, flight => flight.Arrive(DateTime.Now));
    }
}