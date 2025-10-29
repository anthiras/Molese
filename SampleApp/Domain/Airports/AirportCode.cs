using System.Text.Json;
using System.Text.Json.Serialization;
using SampleApp.Domain.Aircrafts;

namespace SampleApp.Domain.Airports;

[JsonConverter(typeof(AirportCodeJsonConverter))]
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

internal class AirportCodeJsonConverter : JsonConverter<AirportCode>
{
    public override AirportCode Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options) => new AirportCode(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, AirportCode value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}