using System.Text.Json;
using System.Text.Json.Serialization;

namespace Andor.Foundation.Contracts.Results;

public class ErrorCodeConverter : JsonConverter<ApplicationErrorCode>
{
    public override ApplicationErrorCode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.StartObject)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            int value = doc.RootElement.GetProperty("value").GetInt32();
            return ApplicationErrorCode.New(value);
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return ApplicationErrorCode.New(reader.GetInt32());
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, ApplicationErrorCode value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value.Value);
    }
}
