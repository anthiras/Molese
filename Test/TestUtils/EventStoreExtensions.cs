using Framework;

namespace Test.TestUtils;

public static class EventStoreExtensions
{
    public static async Task<List<Event>> CaptureEvents(this IEventStore eventStore)
    {
        var publishedEvents = new List<Event>();
        await eventStore.Subscribe((e, _) =>
        {
            publishedEvents.Add(e);
            return Task.CompletedTask;
        });
        return publishedEvents;
    }
}