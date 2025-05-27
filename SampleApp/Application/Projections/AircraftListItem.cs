using SampleApp.Domain.Aircrafts;
using Framework;

namespace SampleApp.Application.Projections;

public class AircraftListItem : Document<AircraftListItem>
{
    public required Id<Aircraft> AircraftId { get; init; }
    public static Id<AircraftListItem> DocId(Id<Aircraft> id) => new(id.Value);
    
    public string? Registration { get; init; }
    public int Flights { get; set; }

}