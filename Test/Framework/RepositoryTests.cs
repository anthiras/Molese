using Domain.Aircrafts;
using Framework;
using InMemory;
using Test.Domain;
using Test.TestUtils;

namespace Test.Framework;

public abstract class RepositoryTests
{
    protected IRepository<Aircraft> Sut = null!;

    [Theory, DomainAutoData]
    public async Task StoreAndFindEntity(Aircraft aircraft)
    {
        await Sut.Store(aircraft);
        var reloaded = await Sut.Find(aircraft.Id);
        
        Assert.Equal(aircraft.Id, reloaded.Id);
    }
}

public class InMemoryRepositoryTests : RepositoryTests
{
    public InMemoryRepositoryTests()
    {
        Sut = new InMemoryRepository<Aircraft>();
    }
}

public class EventSourcedRepositoryTests : RepositoryTests
{
    private readonly InMemoryEventStore _eventStore = new();
    
    public EventSourcedRepositoryTests()
    {
        Sut = new EventSourcedRepository<Aircraft>(_eventStore, (id, events) => new Aircraft(id, events));
    }
    
    [Theory, DomainAutoData]
    public async Task StoringEntityPublishesUncommittedEvents(Aircraft aircraft)
    {
        var uncommittedEvents = aircraft.UncommittedEvents.ToList();

        var publishedEvents = await _eventStore.CaptureEvents();
        
        await Sut.Store(aircraft);
        
        Assert.Equal(publishedEvents, uncommittedEvents);
    }

    [Theory, DomainAutoData]
    public async Task NoEventsAreUncommittedAfterStoringEntity(Aircraft aircraft)
    {
        await Sut.Store(aircraft);
        
        Assert.Empty(aircraft.UncommittedEvents);
    }
    
    [Theory, DomainAutoData]
    public async Task FailsForDuplicates(Id<Aircraft> id, AircraftRegistration registration)
    {
        var aircraft1 = new Aircraft(id, registration);
        var aircraft2 = new Aircraft(id, registration);
        
        await Sut.Store(aircraft1);
        await Assert.ThrowsAnyAsync<Exception>(() => Sut.Store(aircraft2));
    }
}