using Framework;
using NSubstitute;
using SampleApp.Application.Commands;
using SampleApp.Domain.Aircrafts;
using SampleApp.Test.Domain;

namespace SampleApp.Test.Application.Commands;

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