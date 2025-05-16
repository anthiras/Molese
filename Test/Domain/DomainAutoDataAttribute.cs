using AutoFixture.Xunit2;
using Test.Domain.Aircrafts;
using Test.Domain.Flights;

namespace Test.Domain;

public class DomainAutoDataAttribute : AutoDataAttribute
{
    public DomainAutoDataAttribute() : base(() => new AutoFixture.Fixture()
        .Customize(new TimeRangeCustomization())
        .Customize(new AirportCodeCustomization())
        .Customize(new ScheduleCustomization())
        .Customize(new FlightCustomization())
        .Customize(new AircraftCustomization()))
    {
        
    }
}