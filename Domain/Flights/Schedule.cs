using Domain.Time;

namespace Domain.Flights;

public record Schedule(DateTime DepartureTime, DateTime ArrivalTime) : TimeRange(DepartureTime, ArrivalTime)
{
    public new Schedule Add(TimeSpan delay)
    {
        var timeRange = base.Add(delay);
        return new Schedule(timeRange.StartTime, timeRange.EndTime);
    }
}