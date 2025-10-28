namespace Framework;

public class EventSourcedRepository<T>(IEventStore eventStore) 
    : IRepository<T> where T : AggregateRoot<T>
{
    public async Task<T> Find(Id<T> id, CancellationToken ct = default)
    {
        var stream = await eventStore.LoadStream(id, ct);

        return (T?) Activator.CreateInstance(typeof(T), id, stream.Cast<Event<T>>())
            ?? throw new Exception($"Failed to instantiate type {typeof(T).Name}");
    }

    public async Task Store(T entity, CancellationToken ct = default)
    {
        await eventStore.AppendToStream(entity.Id, entity.UncommittedEvents, entity.Version, ct);
        
        entity.ClearEvents();
    }

    public async Task Delete(T entity, CancellationToken ct = default)
    {
        await Store(entity, ct);
        await eventStore.DeleteStream(entity.Id, ct);
    }
}