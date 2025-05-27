using SampleApp.Domain.Aircrafts;
using Framework;

namespace SampleApp.Application.Projections;

public partial class AircraftListView(IRepository<AircraftListItem> repo) : IEventHandler
{
    private Task HandleAsync(AircraftCreated @event, CancellationToken ct = default)
    {
        return repo.Store(new AircraftListItem()
        {
            Id = AircraftListItem.DocId(@event.AircraftId),
            AircraftId = @event.AircraftId,
            Registration = @event.Registration.ToString()
        }, ct);
    }

    private Task HandleAsync(AircraftAssignedToFlight @event, CancellationToken ct = default)
    {
        return repo.Update(AircraftListItem.DocId(@event.AircraftId), x => x.Flights++, ct);
    }

    private Task HandleAsync(AircraftUnassignedFromFlight @event, CancellationToken ct = default)
    {
        return repo.Update(AircraftListItem.DocId(@event.AircraftId), x => x.Flights--, ct);
    }
}