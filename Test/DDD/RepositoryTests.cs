using AutoFixture.Xunit2;
using Framework;
using InMemory;
using Test.TestUtils;

namespace Test.DDD;

public abstract class RepositoryTests(IRepository<TestAggregate> sut)
{
    protected readonly IRepository<TestAggregate> Sut = sut;
    
    [Theory, AutoData]
    public async Task StoreAndFindEntity(TestAggregate aggregate)
    {
        await Sut.Store(aggregate);
        var reloaded = await Sut.Find(aggregate.Id);
        
        Assert.Equal(aggregate.Id, reloaded.Id);
    }
}

public class InMemoryRepositoryTests() 
    : RepositoryTests(new InMemoryRepository<TestAggregate>());

public class EventSourcedRepositoryTests() 
    : RepositoryTests(new EventSourcedRepository<TestAggregate>(EventStore))
{
    private static readonly InMemoryEventStore EventStore = new();
    
    [Theory, AutoData]
    public async Task StoringEntityPublishesUncommittedEvents(TestAggregate aggregate)
    {
        var uncommittedEvents = aggregate.UncommittedEvents.ToList();

        var publishedEvents = await EventStore.CaptureEvents();
        
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