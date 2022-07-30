using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LLNETUtils.Configuration.Serialization;

internal class JsonConfigSerializer : IConfigSerializer
{
    public ConfigSection Deserialize(string data)
    {
        JsonElement document = JsonDocument.Parse(data).RootElement;

        if (document.ValueKind != JsonValueKind.Object)
        {
            throw new ArgumentException("JSON must represent an object type.");
        }

        return (ConfigSection) DeserializeJsonElement(document);
    }

    public string Serialize(ConfigSection section)
    {
        JsonSerializerOptions options = new()
            {WriteIndented = true, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)};
        return JsonSerializer.Serialize(section, options);
    }

    private object DeserializeJsonElement(JsonElement jsonElement)
    {
        switch (jsonElement.ValueKind)
        {
            case JsonValueKind.Object:
                ConfigSection dict = new();

                foreach (JsonProperty property in jsonElement.EnumerateObject())
                {
                    dict.Add(KeyValuePair.Create(property.Name, DeserializeJsonElement(property.Value)));
                }

                return dict;

            case JsonValueKind.Array:
                List<object> list = new();

                foreach (JsonElement arrayElement in jsonElement.EnumerateArray())
                {
                    list.Add(DeserializeJsonElement(arrayElement));
                }

                return list;

            case JsonValueKind.String:
                return jsonElement.GetString()!;

            case JsonValueKind.Number:
                if (jsonElement.TryGetInt32(out int intNum))
                {
                    return intNum;
                }

                return jsonElement.GetDouble();

            case JsonValueKind.True:
                return true;

            case JsonValueKind.False:
                return false;

            default:
                return null!;
        }
    }
}