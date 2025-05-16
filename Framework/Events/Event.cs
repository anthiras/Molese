namespace Framework;

public abstract record Event(StreamId StreamId);

// public record AggregateEvent<TAggregate, TEvent>(Id<TAggregate> Id, TEvent Event) : Event where TEvent : Event;
//
// public record StoredEvent(string StreamId, Event Event);