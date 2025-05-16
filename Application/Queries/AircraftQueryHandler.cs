using Application.Projections;
using Domain.Aircrafts;
using Framework;

namespace Application.Queries;

public class AircraftQueryHandler(IDocumentStore store) : 
    IQueryHandler<GetAllAircrafts, IAsyncEnumerable<AircraftListItem>>,
    IQueryHandler<GetAircraftById, Task<AircraftListItem>>
{
    public IAsyncEnumerable<AircraftListItem> Query(GetAllAircrafts query, CancellationToken ct = default)
    {
        return store.FindAll<AircraftListItem>(ct);
    }

    public Task<AircraftListItem> Query(GetAircraftById query, CancellationToken ct = default)
    {
        return store.Find<AircraftListItem, Aircraft>(query.Id, ct);
    }
}