using System.Text.Json;
using System.Text.Json.Serialization;

namespace Eventflow.Domain.Common
{
    public class StatesConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Try reading as an array first
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                var list = JsonSerializer.Deserialize<List<object>>(ref reader);
                return string.Join(", ", list.Select(x => x.ToString()));
            }
            // Otherwise, treat it as a regular string
            else if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }

            // Default case: return a blank string
            return string.Empty;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            // Serialize as string
            writer.WriteStringValue(value);
        }
    }
}
