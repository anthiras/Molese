using Framework;

namespace Test.TestUtils;

/// <summary>
/// A wrapper for IEventStore that tracks the number of received events for test purposes
/// </summary>
/// <param name="eventStore">The underlying event store</param>
public class TrackedEventStore(IEventStore eventStore) : IEventStore
{
    private int _eventsReceived;


    public Task Subscribe(Func<Event, CancellationToken, Task> callback, CancellationToken ct = default)
    {
        return eventStore.Subscribe(async (@event, token) =>
        {
            await callback(@event, token);
            Interlocked.Increment(ref _eventsReceived);
        }, ct);
    }

    public async Task WaitForEvents(int count)
    {
        int attempts = 0;
        while (_eventsReceived < count && attempts++ < 100)
            await Task.Delay(10);
    }

    public Task<IEnumerable<Event>> LoadStream(StreamId streamId, CancellationToken ct = default) =>
        eventStore.LoadStream(streamId, ct);

    public Task AppendToStream(StreamId streamId, IEnumerable<Event> events, int expectedVersion,
        CancellationToken ct = default) =>
        eventStore.AppendToStream(streamId, events, expectedVersion, ct);

    public Task DeleteStream(StreamId streamId, CancellationToken ct = default) =>
        eventStore.DeleteStream(streamId, ct);
}