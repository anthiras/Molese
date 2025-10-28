using SampleApp.Domain.Aircrafts;
using Framework;

namespace SampleApp.Application.Commands;

public class AircraftCommandHandler(IRepository<Aircraft> repository) : 
    ICommandHandler<CreateAircraft>,
    ICommandHandler<DeleteAircraft>
{
    public async Task Handle(CreateAircraft command)
    {
        var aircraft = new Aircraft(Id<Aircraft>.New(), command.Registration);

        await repository.Store(aircraft);
    }

    public async Task Handle(DeleteAircraft command)
    {
        var aircraft = await repository.Find(command.Id);

        aircraft.Delete();
        
        await repository.Store(aircraft);
    }
}