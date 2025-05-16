namespace Domain.Time;

public record TimeRange
{
    public TimeRange(DateTime startTime, DateTime endTime)
    {
        if (endTime < startTime)
            throw new ArgumentOutOfRangeException(nameof(endTime),
                "End time cannot be earlier than start time");
        
        StartTime = startTime;
        EndTime = endTime;
    }

    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public TimeRange Add(TimeSpan delay)
    {
        return new TimeRange(StartTime + delay, EndTime + delay);
    }

    public bool Overlaps(TimeRange other)
    {
        return other.StartTime < EndTime && other.EndTime > StartTime; 
    }
}