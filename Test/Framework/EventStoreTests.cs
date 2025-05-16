using System.Data;
using AutoFixture.Xunit2;
using EventStore.Client;
using Framework;
using InMemory;
using Kurrent;
using Test.Kurrent;
using Test.TestUtils;
using Testcontainers.EventStoreDb;

namespace Test.Framework;

public abstract class EventStoreTests
{
    private readonly TrackedEventStore _sut;

    protected EventStoreTests(IEventStore eventStore)
    {
        _sut = new TrackedEventStore(eventStore);
    }
    
    [Theory, AutoData]
    public async Task AppendedEventsAreReloadedInOrder(StreamId streamId, TestEvent[] events)
    {
        await _sut.AppendToStream(streamId, events[..2], 0);
        await _sut.AppendToStream(streamId, events[2..], 2);

        var reloaded = await _sut.LoadStream(streamId);
        
        Assert.Equal(events, reloaded);
    }

    [Theory, AutoData]
    public async Task AppendFailsIfExpectedVersionDoesntMatchNumberOfStoredEvents(StreamId streamId, TestEvent[] events)
    {
        await Assert.ThrowsAsync<DBConcurrencyException>(() => _sut.AppendToStream(streamId, events, 1));
    }

    [Theory, AutoData]
    public async Task AppendedEventsArePublishedToSubscriber(StreamId streamId, TestEvent[] events)
    {
        var publishedEvents = await _sut.CaptureEvents();
        
        await _sut.AppendToStream(streamId, events, 0);

        await _sut.WaitForEvents(events.Length);
        
        Assert.Equivalent(events, publishedEvents);
    }
}

public class InMemoryEventStoreTests() : EventStoreTests(new InMemoryEventStore());

public class KurrentEventStoreTests(KurrentContainerFixture fixture) :
    EventStoreTests(new KurrentEventStore(
        new EventStoreClient(EventStoreClientSettings.Create(fixture.Container.GetConnectionString())))),
    IClassFixture<KurrentContainerFixture>;


public record TestEvent(string Message) : Event(new StreamId("1234"));