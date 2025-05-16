using Application;
using Application.Projections;
using Domain;
using Domain.Aircrafts;
using EventStore.Client;
using Framework;
using InMemory;
using Kurrent;
using Microsoft.Extensions.DependencyInjection;
using Test.Domain;
using Test.Kurrent;
using Test.TestUtils;

namespace Test.Application;

public abstract class ProjectionTests
{
    private readonly IRepository<Aircraft, Id<Aircraft>> _repository;
    private readonly IDocumentStore _documentStore;
    private readonly ProjectionSubscriber _subscriber;
    private readonly TrackedEventStore _trackedEventStore;
    
    protected ProjectionTests(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection()
            .AddDomain()
            .AddApplication();
        configureServices(services);
        services.Decorate<IEventStore, TrackedEventStore>();
        
        var serviceProvider = services.BuildServiceProvider();
        _repository = serviceProvider.GetRequiredService<IRepository<Aircraft, Id<Aircraft>>>();
        _documentStore = serviceProvider.GetRequiredService<IDocumentStore>();
        _subscriber = serviceProvider.GetRequiredService<ProjectionSubscriber>();
        _trackedEventStore = (TrackedEventStore) serviceProvider.GetRequiredService<IEventStore>();
    }

    [Theory, DomainAutoData]
    public async Task AircraftChangesAreProjectedToAircraftView(Id<Aircraft> id, AircraftRegistration registration, Assignment assignment)
    {
        await _subscriber.Subscribe();
        
        var aircraft = new Aircraft(id, registration);
        await _repository.Store(aircraft);

        await _trackedEventStore.WaitForEvents(1);
        
        var listItem = await _documentStore.Find<AircraftListItem, Aircraft>(aircraft.Id);
        Assert.Equal(registration.ToString(), listItem.Registration);
        Assert.Equal(0, listItem.Flights);

        aircraft = await _repository.Find(id);
        aircraft.Assign(assignment);
        await _repository.Store(aircraft);
        
        await _trackedEventStore.WaitForEvents(2);
        
        listItem = await _documentStore.Find<AircraftListItem, Aircraft>(aircraft.Id);
        Assert.Equal(1, listItem.Flights);
    }
}

public class InMemoryProjectionTests() : ProjectionTests(services => services
    .AddInMemoryEventStore()
    //.AddInMemoryPubSub()
    .AddInMemoryDocumentStore()
);

public class KurrentProjectionTests(KurrentContainerFixture fixture) : ProjectionTests(services =>
{
    var settings = EventStoreClientSettings.Create(fixture.Container.GetConnectionString());
    services.AddKurrentEventStore(settings);
    //services.AddKurrentPubSub(settings);
    services.AddInMemoryDocumentStore();
}), IClassFixture<KurrentContainerFixture>;