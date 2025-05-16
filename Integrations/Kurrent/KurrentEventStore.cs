using System.Data;
using EventStore.Client;
using Framework;

namespace Kurrent;

public class KurrentEventStore(EventStoreClient client) : IEventStore
{
    public async Task<IEnumerable<Event>> LoadStream(StreamId streamId, CancellationToken ct = default)
    {
        var result = client.ReadStreamAsync(Direction.Forwards, streamId.ToString(), StreamPosition.Start, cancellationToken: ct);
        var events = await result.ToListAsync(ct);
        return events.Select(e => EventSerializer.Deserialize(e.Event.Data.Span, Type.GetType(e.Event.EventType) ??
            throw new Exception($"Unable to resolve type {e.Event.EventType}")));
    }

    public async Task AppendToStream(StreamId streamId, IEnumerable<Event> events, int expectedVersion, CancellationToken ct = default)
    {
        var data = events.Select(e =>
            new EventData(Uuid.NewUuid(), e.GetType().AssemblyQualifiedName!, EventSerializer.Serialize(e)));
        var revision = expectedVersion == 0 ? StreamRevision.None : StreamRevision.FromInt64(expectedVersion - 1);
        try
        {
            await client.AppendToStreamAsync(streamId.ToString(), revision, data, cancellationToken: ct);
        }
        catch (WrongExpectedVersionException e)
        {
            throw new DBConcurrencyException(e.Message, e);
        }
    }

    public async Task DeleteStream(StreamId streamId, CancellationToken ct = default)
    {
        await client.DeleteAsync(streamId.ToString(), StreamState.Any, cancellationToken: ct);
    }

    public async Task Subscribe(Func<Event, CancellationToken, Task> callback, CancellationToken ct = default)
    {
        await client.SubscribeToAllAsync(FromAll.End, (_, e, token) =>
        {
            var @event = EventSerializer.Deserialize(e.Event.Data.Span,
                Type.GetType(e.Event.EventType) ??
                throw new Exception($"Unable to resolve type {e.Event.EventType}"));
            return callback(@event, token);
        }, filterOptions: new SubscriptionFilterOptions(EventTypeFilter.ExcludeSystemEvents()), cancellationToken: ct);
    }
}