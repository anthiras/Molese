using System.Data;
using Framework;

namespace InMemory;

public class InMemoryEventStore : IEventStore
{
    private readonly Dictionary<StreamId, List<Event>> _eventStreams = new();
    private readonly List<Func<Event, CancellationToken, Task>> _subscriptions = [];
    
    public async Task Publish(Event @event, CancellationToken ct = default)
    {
        foreach (var callback in _subscriptions)
        {
            await callback(@event, ct);
        }
    }

    public Task Subscribe(Func<Event, CancellationToken, Task> callback, CancellationToken ct = default)
    {
        _subscriptions.Add(callback);
        return Task.CompletedTask;
    }
    
    public Task<IEnumerable<Event>> LoadStream(StreamId streamId, CancellationToken ct = default)
    {
        return Task.FromResult(_eventStreams[streamId].AsEnumerable());
    }

    public Task AppendToStream(StreamId streamId, IEnumerable<Event> events, int expectedVersion, CancellationToken ct = default)
    {
        _eventStreams.TryAdd(streamId, []);
        if (_eventStreams[streamId].Count != expectedVersion)
            throw new DBConcurrencyException();
        var eventsArr = events.ToArray();
        _eventStreams[streamId].AddRange(eventsArr);
        return Task.Run(async () =>
        {
            foreach (var @event in eventsArr)
            {
                await Publish(@event, ct);
            }
        }, ct);
    }
}