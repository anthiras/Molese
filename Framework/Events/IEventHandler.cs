namespace Framework;

public interface IEventHandler<in TEvent> where TEvent : Event
{
    void Handle(TEvent @event);
}

public interface IEventHandler
{
    // void Handle(Event @event);
    Task HandleAsync(Event @event, CancellationToken ct = default);
}