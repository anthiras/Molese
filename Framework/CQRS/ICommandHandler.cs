namespace Framework;

public interface ICommandHandler<in TCommand>
{
    Task Handle(TCommand command);
}