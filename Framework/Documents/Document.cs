namespace Framework;

public abstract class Document<T> : IIdentifiable<Id<T>>
{
    public required Id<T> Id { get; init; }
}