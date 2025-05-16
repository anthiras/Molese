using AutoFixture;
using Domain.Flights;
using Domain.Time;

namespace Test.Domain.Flights;

public class ScheduleCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register((TimeRange tr) => new Schedule(tr.StartTime, tr.EndTime));
    }
}