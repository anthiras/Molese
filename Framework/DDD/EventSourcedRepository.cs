namespace Framework;

public class EventSourcedRepository<T>(IEventStore eventStore, Func<Id<T>, IEnumerable<Event<T>>, T> factory) 
    : IRepository<T> where T : AggregateRoot<T>
{
    public async Task<T> Find(Id<T> id, CancellationToken ct = default)
    {
        var stream = await eventStore.LoadStream(new StreamId(id.Value), ct);
        
        return factory(id, stream.Cast<Event<T>>());
    }

    public async Task Store(T entity, CancellationToken ct = default)
    {
        await eventStore.AppendToStream(new StreamId(entity.Id.Value), entity.UncommittedEvents, entity.Version, ct);
        
        entity.ClearEvents();
    }

    public Task Delete(Id<T> id, CancellationToken ct = default)
    {
        return eventStore.DeleteStream(new StreamId(id.Value), ct);
    }
}