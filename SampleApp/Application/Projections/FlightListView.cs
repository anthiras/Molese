using Framework;
using SampleApp.Domain.Flights;

namespace SampleApp.Application.Projections;

public partial class FlightListView(IRepository<FlightListItem> repo) : IEventHandler
{
    private Task HandleAsync(FlightScheduled @event, CancellationToken ct = default)
    {
        return repo.Store(new FlightListItem()
        {
            Id = FlightListItem.DocId(@event.FlightId),
            FlightId = @event.FlightId,
            Route = @event.Route,
            OriginalSchedule = @event.Schedule,
            Status = FlightStatus.Scheduled
        }, ct);
    }

    private Task HandleAsync(FlightAircraftAssigned @event, CancellationToken ct = default)
    {
        return repo.Update(FlightListItem.DocId(@event.FlightId), x => x.AircraftId = @event.AircraftId, ct);
    }

    private Task HandleAsync(FlightDelayed @event, CancellationToken ct = default)
    {
        return repo.Update(FlightListItem.DocId(@event.FlightId), x =>
        {
            x.Status = FlightStatus.Delayed;
            x.NewSchedule = @event.NewSchedule;
        }, ct);
    }
    
    private Task HandleAsync(FlightDeparted @event, CancellationToken ct = default)
    {
        return repo.Update(FlightListItem.DocId(@event.FlightId), x =>
        {
            x.Status = FlightStatus.Departed;
            x.ActualDepartureTime = @event.DepartureTime;
        }, ct);
    }
    
    private Task HandleAsync(FlightArrived @event, CancellationToken ct = default)
    {
        return repo.Update(FlightListItem.DocId(@event.FlightId), x =>
        {
            x.Status = FlightStatus.Arrived;
            x.ActualArrivalTime = @event.ArrivalTime;
        }, ct);
    }
}