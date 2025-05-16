namespace Framework;

public abstract class EventSourced<TAggregate>
{
    public int Version { get; private set; }
    
    protected EventSourced() {}
    
    protected EventSourced(IEnumerable<Event<TAggregate>> events)
    {
        Apply(events);
    }
    
    protected abstract void Apply(Event<TAggregate> @event);

    private void Apply(IEnumerable<Event<TAggregate>> events)
    {
        foreach (var @event in events)
        {
            Apply(@event);
            Version++;
        }
    }
}