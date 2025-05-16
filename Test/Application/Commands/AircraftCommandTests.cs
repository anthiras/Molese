using Application.Commands;
using Domain.Aircrafts;
using Framework;
using InMemory;
using Test.Domain;

namespace Test.Application.Commands;

public class AircraftCommandTests
{
    private readonly AircraftCommandHandler _sut;
    private readonly InMemoryRepository<Aircraft, Id<Aircraft>> _repository = new();

    public AircraftCommandTests()
    {
        _sut = new AircraftCommandHandler(_repository);
    }

    [Theory, DomainAutoData]
    public async Task CreateCommandStoresNewAircraftInRepo(CreateAircraft command)
    {
        var id = await _sut.Handle(command);

        var aircraft = await _repository.Find(id);
        
        Assert.Equal(command.Registration, aircraft.Registration);
    }
}