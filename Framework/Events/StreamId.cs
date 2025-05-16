namespace Framework;

public readonly record struct StreamId(string Value)
{
    public override string ToString() => Value;
}