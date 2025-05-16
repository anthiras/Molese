using System.Text.Json;

namespace Framework;

public static class EventSerializer
{
    public static byte[] Serialize(Event e)
    {
        return JsonSerializer.SerializeToUtf8Bytes(e, e.GetType());
    }

    public static Event Deserialize(ReadOnlySpan<byte> data, Type type)
    {
        return (Event)(JsonSerializer.Deserialize(data, type) ?? throw new Exception("Deserialization failed"));
    }
}