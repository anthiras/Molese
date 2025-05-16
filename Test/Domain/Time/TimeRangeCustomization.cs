using AutoFixture;
using Domain.Time;

namespace Test.Domain;

public class TimeRangeCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Register((DateTime d1, DateTime d2) => new TimeRange(d1 < d2 ? d1 : d2, d1 < d2 ? d2 : d1));
    }
}