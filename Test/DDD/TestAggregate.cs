using Framework;

namespace Test.DDD;

public partial class TestAggregate : AggregateRoot<TestAggregate>
{
    public TestAggregate(Id<TestAggregate> id) : base(id)
    {
        Raise(new TestAggregateCreated(Id));
    }

    public TestAggregate(Id<TestAggregate> id, IEnumerable<Event<TestAggregate>> events) : base(id, events)
    {
    }

    public void Apply(TestAggregateCreated @event)
    {
        
    }
}

public record TestAggregateCreated(Id<TestAggregate> Id) : Event<TestAggregate>(Id);
