using AutoFixture.Xunit2;
using SampleApp.Test.Domain.Aircrafts;
using SampleApp.Test.Domain.Airports;
using SampleApp.Test.Domain.Flights;
using SampleApp.Test.Domain.Time;

namespace SampleApp.Test.Domain;

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