using Application.Commands;
using Domain.Aircrafts;
using Framework;
using InMemory;
using NSubstitute;
using Test.Domain;

namespace Test.Application.Commands;

public class AircraftCommandTests
{
    private readonly AircraftCommandHandler _sut;
    private readonly IRepository<Aircraft> _repository = Substitute.For<IRepository<Aircraft>>();

    public AircraftCommandTests()
    {
        _sut = new AircraftCommandHandler(_repository);
    }

    [Theory, DomainAutoData]
    public async Task CreateCommandStoresNewAircraftInRepo(CreateAircraft command)
    {
        await _sut.Handle(command);

        await _repository.Received().Store(Arg.Is<Aircraft>(a => command.Registration.Equals(a.Registration)));
    }
}