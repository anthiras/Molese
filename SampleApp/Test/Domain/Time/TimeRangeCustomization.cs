using AutoFixture;
using SampleApp.Domain.Time;

namespace SampleApp.Test.Domain.Time;

public class TimeRangeCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register((DateTime d1, DateTime d2) => new TimeRange(d1 < d2 ? d1 : d2, d1 < d2 ? d2 : d1));
    }
}