using Domain.Airports;

namespace Domain.Flights;

public record Route(AirportCode Origin, AirportCode Destination);