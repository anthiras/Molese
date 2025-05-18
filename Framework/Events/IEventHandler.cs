namespace Framework;

public interface IEventHandler
{
    Task HandleAsync(Event @event, CancellationToken ct = default);
}