namespace Framework;

public abstract class AggregateRoot<T> : EventSourced<T>, IIdentifiable<Id<T>>
{
    protected AggregateRoot(Id<T> id)
    {
        Id = id;
    }

    protected AggregateRoot(Id<T> id, IEnumerable<Event<T>> events) : base(events)
    {
        Id = id;
    }

    public Id<T> Id { get; }

    private readonly List<Event<T>> _uncommitedEvents = [];
    public IEnumerable<Event<T>> UncommittedEvents => _uncommitedEvents;

    public void ClearEvents()
    {
        _uncommitedEvents.Clear();
    }

    protected void Raise(Event<T> @event)
    {
        _uncommitedEvents.Add(@event);
        Apply(@event);
    }
}