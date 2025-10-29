using Framework;
using SampleApp.Application.Projections;

namespace SampleApp.Application.Queries;

public class FlightQueryHandler(IDocumentStore<FlightListItem> store) : 
    IQueryHandler<GetAllFlights, IAsyncEnumerable<FlightListItem>>
{
    public IAsyncEnumerable<FlightListItem> Query(GetAllFlights query, CancellationToken ct = default)
    {
        return store.FindAll(ct);
    }
}