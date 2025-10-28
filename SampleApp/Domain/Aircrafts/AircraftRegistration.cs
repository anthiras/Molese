using System.Text.Json;
using System.Text.Json.Serialization;

namespace SampleApp.Domain.Aircrafts;

[JsonConverter(typeof(AircraftRegistrationJsonConverter))]
public readonly struct AircraftRegistration(string value)
{
    public string Value { get; } = value;

    public override string ToString() => Value;
}

internal class AircraftRegistrationJsonConverter : JsonConverter<AircraftRegistration>
{
    public override AircraftRegistration Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options) => new AircraftRegistration(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, AircraftRegistration value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.Value);
}