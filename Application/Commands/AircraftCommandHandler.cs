using Domain.Aircrafts;
using Framework;

namespace Application.Commands;

public class AircraftCommandHandler
{
    private readonly IRepository<Aircraft, Id<Aircraft>> _repository;

    public AircraftCommandHandler(IRepository<Aircraft, Id<Aircraft>> repository)
    {
        _repository = repository;
    }

    public async Task<Id<Aircraft>> Handle(CreateAircraft command)
    {
        var aircraft = new Aircraft(Id<Aircraft>.New(), command.Registration);

        await _repository.Store(aircraft);

        return aircraft.Id;
    }
}