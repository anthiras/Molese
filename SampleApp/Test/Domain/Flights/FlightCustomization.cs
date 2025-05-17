using AutoFixture;
using Framework;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;
using Test.TestUtils;

namespace SampleApp.Test.Domain.Flights;

public class FlightCustomization() : InlineCustomization(fixture =>
    fixture.Register((Id<Flight> flightId, Route route, Schedule schedule) =>
        new Flight(flightId, route, schedule)
    ));

public class AssignedAttribute() : InlineCustomizationAttribute(fixture => 
    fixture.Register((Id<Flight> flightId, Route route, Schedule schedule, Id<Aircraft> aircraftId) =>
    {
        var flight = new Flight(flightId, route, schedule);
        flight.Assign(aircraftId);
        return flight;
    }));
    
public class DepartedAttribute() : InlineCustomizationAttribute(fixture => 
    fixture.Register((Id<Flight> flightId, Route route, Schedule schedule, Id<Aircraft> aircraftId) =>
    {
        var flight = new Flight(flightId, route, schedule);
        flight.Assign(aircraftId);
        flight.Depart(flight.Schedule!.DepartureTime);
        return flight;
    }));