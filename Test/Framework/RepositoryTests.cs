using AutoFixture.Xunit2;
using Framework;
using InMemory;
using Test.TestUtils;

namespace Test.Framework;

public abstract class RepositoryTests
{
    protected IRepository<TestAggregate> Sut = null!;

    [Theory, AutoData]
    public async Task StoreAndFindEntity(TestAggregate aggregate)
    {
        await Sut.Store(aggregate);
        var reloaded = await Sut.Find(aggregate.Id);
        
        Assert.Equal(aggregate.Id, reloaded.Id);
    }
}

public class InMemoryRepositoryTests : RepositoryTests
{
    public InMemoryRepositoryTests()
    {
        Sut = new InMemoryRepository<TestAggregate>();
    }
}

public class EventSourcedRepositoryTests : RepositoryTests
{
    private readonly InMemoryEventStore _eventStore = new();
    
    public EventSourcedRepositoryTests()
    {
        Sut = new EventSourcedRepository<TestAggregate>(_eventStore, (id, events) => new TestAggregate(id, events));
    }
    
    [Theory, AutoData]
    public async Task StoringEntityPublishesUncommittedEvents(TestAggregate aggregate)
    {
        var uncommittedEvents = aggregate.UncommittedEvents.ToList();

        var publishedEvents = await _eventStore.CaptureEvents();
        
        await Sut.Store(aggregate);
        
        Assert.Equal(publishedEvents, uncommittedEvents);
    }

    [Theory, AutoData]
    public async Task NoEventsAreUncommittedAfterStoringEntity(TestAggregate aggregate)
    {
        await Sut.Store(aggregate);
        
        Assert.Empty(aggregate.UncommittedEvents);
    }
    
    [Theory, AutoData]
    public async Task FailsForDuplicates(Id<TestAggregate> id)
    {
        var aggregate1 = new TestAggregate(id);
        var aggregate2 = new TestAggregate(id);
        
        await Sut.Store(aggregate1);
        await Assert.ThrowsAnyAsync<Exception>(() => Sut.Store(aggregate2));
    }
}