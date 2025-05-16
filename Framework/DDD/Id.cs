using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Framework;

[JsonConverter(typeof(IdJsonConverterFactory))]
public readonly record struct Id<T>(string Value) : IParsable<Id<T>>
{
    public static Id<T> New() => new(Guid.NewGuid().ToString());

    public override string ToString() => Value;
    
    public static Id<T> Parse(string s, IFormatProvider? provider) => new(s);

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Id<T> result)
    {
        result = s != null ? new Id<T>(s) : default;
        return s != null;
    }
}

public class IdJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Id<>);
    }
    
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var elementType = typeToConvert.GetGenericArguments()[0];
        return (JsonConverter?)Activator.CreateInstance(typeof(IdJsonConverter<>).MakeGenericType(elementType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null);
    }
    
    private class IdJsonConverter<T> : JsonConverter<Id<T>>
    {
        public override Id<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new Id<T>(reader.GetString()!);
        }

        public override void Write(Utf8JsonWriter writer, Id<T> value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Value);
        }
    }
}
    