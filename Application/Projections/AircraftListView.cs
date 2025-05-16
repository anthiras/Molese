using Domain.Aircrafts;
using Framework;

namespace Application.Projections;

public partial class AircraftListView : IEventHandler
{
    private readonly IDocumentStore _store;

    public AircraftListView(IDocumentStore store)
    {
        _store = store;
    }

    private Task HandleAsync(AircraftCreated @event, CancellationToken ct = default)
    {
        return _store.Store<AircraftListItem, Aircraft>(new AircraftListItem()
        {
            Id = @event.AircraftId,
            Registration = @event.Registration.ToString()
        }, ct);
    }

    private Task HandleAsync(AircraftAssignedToFlight @event, CancellationToken ct = default)
    {
        return _store.Update<AircraftListItem, Aircraft>(@event.AircraftId, x => x.Flights++, ct);
    }

    private Task HandleAsync(AircraftUnassignedFromFlight @event, CancellationToken ct = default)
    {
        return _store.Update<AircraftListItem, Aircraft>(@event.AircraftId, x => x.Flights--, ct);
    }
}