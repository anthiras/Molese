using Framework;
using SampleApp.Domain.Aircrafts;

namespace SampleApp.Api.Flights;

public record UpdateFlight(Id<Aircraft>? AircraftId, DateTime? DepartureTime);