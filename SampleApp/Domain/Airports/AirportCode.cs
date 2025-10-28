namespace SampleApp.Domain.Airports;

public readonly struct AirportCode
{
    public AirportCode(string code)
    {
        if (code.Length != 3)
            throw new ArgumentException("Airport code must be exactly 3 characters long");
        if (!code.All(char.IsUpper))
            throw new ArgumentException("Airport code must be uppercase");
        Value = code;
    }
    
    public string Value { get; }
    
    public override string ToString() => Value;
}