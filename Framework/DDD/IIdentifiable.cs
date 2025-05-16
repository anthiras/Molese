namespace Framework;

public interface IIdentifiable<out TId>
{
    TId Id { get; }
}