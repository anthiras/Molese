using Framework;

namespace Application.Projections;

public class ProjectionSubscriber(IEventStore eventStore, IEnumerable<IEventHandler> eventHandlers)
{
    public async Task Subscribe(CancellationToken ct = default)
    {
        await eventStore.Subscribe(
            async (@event, ct1) =>
                await Task.WhenAll(eventHandlers.Select(handler => handler.HandleAsync(@event, ct1))), ct);
    }
}