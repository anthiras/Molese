using EventStore.Client;
using Framework;
using InMemory;
using Kurrent;
using Microsoft.Extensions.DependencyInjection;
using Mongo;
using MongoDB.Driver;
using SampleApp.Application;
using SampleApp.Application.Projections;
using SampleApp.Domain;
using SampleApp.Domain.Aircrafts;
using SampleApp.Domain.Flights;
using SampleApp.Domain.Time;
using SampleApp.Test.Domain;
using Test.Kurrent;
using Test.TestUtils;

namespace SampleApp.Test.Application.Projections;

public abstract class ProjectionTests
{
    private readonly IDocumentStore<AircraftListItem> _documentStore;
    private readonly EventSubscriber _subscriber;
    private readonly TrackedEventStore _trackedEventStore;
    
    protected ProjectionTests(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection()
            .RegisterHandlersFromAssemblies(typeof(Aircraft).Assembly, typeof(AircraftListItem).Assembly)
            .RegisterEventSourcedRepositories(typeof(Aircraft).Assembly)
            .RegisterDocumentRepositories(typeof(AircraftListItem).Assembly)
            .AddEventSubscriber();
        configureServices(services);
        services.Decorate<IEventStore, TrackedEventStore>();
        
        var serviceProvider = services.BuildServiceProvider();
        _documentStore = serviceProvider.GetRequiredService<IDocumentStore<AircraftListItem>>();
        _subscriber = serviceProvider.GetRequiredService<EventSubscriber>();
        _trackedEventStore = (TrackedEventStore) serviceProvider.GetRequiredService<IEventStore>();
    }

    [Theory, DomainAutoData]
    public async Task AircraftEventsAreProjectedToAircraftView(Id<Aircraft> id, AircraftRegistration registration, Id<Flight> flightId, TimeRange timeRange)
    {
        await _subscriber.Subscribe();
        
        await _trackedEventStore.AppendToStream(id, [new AircraftCreated(id, registration)], 0);

        await _trackedEventStore.WaitForEvents(1);
        
        var listItem = await _documentStore.Find(AircraftListItem.DocId(id));
        Assert.Equal(registration.ToString(), listItem.Registration);
        Assert.Equal(0, listItem.Flights);

        await _trackedEventStore.AppendToStream(id, [new AircraftAssignedToFlight(id, flightId, timeRange)], 1);
        
        await _trackedEventStore.WaitForEvents(2);
        
        listItem = await _documentStore.Find(AircraftListItem.DocId(id));
        Assert.Equal(1, listItem.Flights);
    }
}

public class InMemoryProjectionTests() : ProjectionTests(services => services
    .AddInMemoryEventStore()
    .AddInMemoryDocumentStore()
);

public class KurrentProjectionTests(KurrentContainerFixture fixture) : ProjectionTests(services =>
{
    var settings = EventStoreClientSettings.Create(fixture.Container.GetConnectionString());
    services.AddKurrentEventStore(settings);
    services.AddInMemoryDocumentStore();
}), IClassFixture<KurrentContainerFixture>;

public class MongoDbProjectionTests(MongoDbContainerFixture fixture) : ProjectionTests(services =>
    services.AddMongoDbDocumentStore(MongoClientSettings.FromConnectionString(fixture.Container.GetConnectionString()))
        .AddInMemoryEventStore()
    ), IClassFixture<MongoDbContainerFixture>;