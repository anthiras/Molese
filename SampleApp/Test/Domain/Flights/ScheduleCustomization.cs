using AutoFixture;
using SampleApp.Domain.Flights;
using SampleApp.Domain.Time;

namespace SampleApp.Test.Domain.Flights;

public class ScheduleCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register((TimeRange tr) => new Schedule(tr.StartTime, tr.EndTime));
    }
}