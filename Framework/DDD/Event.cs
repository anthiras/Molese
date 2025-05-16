namespace Framework;

public record Event<TAggregate>(Id<TAggregate> AggregateId) : Event(new StreamId(AggregateId.Value));