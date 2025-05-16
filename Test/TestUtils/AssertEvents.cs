using Framework;

namespace Test;

public static class AssertEvents
{
    public static void Raised<T>(Event @event, AggregateRoot<T> aggregateRoot)
    {
        Assert.Contains(@event, aggregateRoot.UncommittedEvents);
    }
}