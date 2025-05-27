using SampleApp.Domain.Aircrafts;
using Framework;
using SampleApp.Application.Projections;

namespace SampleApp.Application.Queries;

public class AircraftQueryHandler(IDocumentStore<AircraftListItem> store) : 
    IQueryHandler<GetAllAircrafts, IAsyncEnumerable<AircraftListItem>>,
    IQueryHandler<GetAircraftById, Task<AircraftListItem>>
{
    public IAsyncEnumerable<AircraftListItem> Query(GetAllAircrafts query, CancellationToken ct = default)
    {
        return store.FindAll(ct);
    }

    public Task<AircraftListItem> Query(GetAircraftById query, CancellationToken ct = default)
    {
        return store.Find(AircraftListItem.DocId(query.AircraftId), ct);
    }
}