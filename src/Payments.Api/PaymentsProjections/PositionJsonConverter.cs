using System.Text.Json;
using System.Text.Json.Serialization;
using EventStore.Client;

namespace Payments.Api.PaymentsProjections;

public class PositionJsonConverter : JsonConverter<Position>
{
    public override Position Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        ulong c = 0, p = 0;

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            if (reader.ValueTextEquals("C"))
            {
                reader.Read();
                c = reader.GetUInt64();
                continue;
            }

            if (reader.ValueTextEquals("P"))
            {
                reader.Read();
                p = reader.GetUInt64();
                continue;
            }
        }

        return new Position(commitPosition: c, preparePosition: p);
    }

    public override void Write(
        Utf8JsonWriter writer,
        Position position,
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("C");
        writer.WriteNumberValue(position.CommitPosition);
        writer.WritePropertyName("P");
        writer.WriteNumberValue(position.PreparePosition);
        writer.WriteEndObject();
    }
}