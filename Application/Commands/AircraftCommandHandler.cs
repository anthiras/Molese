using Domain.Aircrafts;
using Framework;

namespace Application.Commands;

public class AircraftCommandHandler(IRepository<Aircraft> repository) : 
    ICommandHandler<CreateAircraft>,
    ICommandHandler<DeleteAircraft>
{
    public async Task Handle(CreateAircraft command)
    {
        var aircraft = new Aircraft(Id<Aircraft>.New(), command.Registration);

        await repository.Store(aircraft);
    }

    public Task Handle(DeleteAircraft command)
    {
        return repository.Delete(command.Id);
    }
}