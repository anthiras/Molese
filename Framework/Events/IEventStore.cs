namespace Framework;

public interface IEventStore
{
    Task<IEnumerable<Event>> LoadStream(StreamId streamId, CancellationToken ct = default);
    Task AppendToStream(StreamId streamId, IEnumerable<Event> events, int expectedVersion, CancellationToken ct = default);
    Task DeleteStream(StreamId streamId, CancellationToken ct = default);
    Task Subscribe(Func<Event, CancellationToken, Task> callback, CancellationToken ct = default);
}