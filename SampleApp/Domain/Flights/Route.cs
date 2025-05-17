using SampleApp.Domain.Airports;

namespace SampleApp.Domain.Flights;

public record Route(AirportCode Origin, AirportCode Destination);