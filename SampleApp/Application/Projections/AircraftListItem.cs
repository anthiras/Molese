using SampleApp.Domain.Aircrafts;
using Framework;

namespace SampleApp.Application.Projections;

public class AircraftListItem : IIdentifiable<Id<Aircraft>>
{
    public required Id<Aircraft> Id { get; set; }
    public string? Registration { get; set; }
    public int Flights { get; set; }
}