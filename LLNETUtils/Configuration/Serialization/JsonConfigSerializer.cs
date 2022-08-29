using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace LLNETUtils.Configuration.Serialization;

internal class JsonConfigSerializer : IConfigSerializer
{
    public ConfigSection Deserialize(string data)
    {
        JsonSerializerOptions options = new() {Converters = { new ConfigSectionConverter() }};
        ConfigSection result = JsonSerializer.Deserialize<ConfigSection>(data, options)!;

        return result;
    }

    public string Serialize(ConfigSection section)
    {
        JsonSerializerOptions options = new()
        {
            Converters = { new ConfigSectionConverter() },
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        return JsonSerializer.Serialize(section, options);
    }
    
    private class ConfigSectionConverter : JsonConverter<ConfigSection>
    {
        public override ConfigSection Read(ref Utf8JsonReader reader, Type? typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException($"JsonTokenType was of type {reader.TokenType}, only objects are supported");
            }

            ConfigDictionary dictionary = new();
            
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return new ConfigSection(dictionary);
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException("JsonTokenType was not PropertyName");
                }

                string? propertyName = reader.GetString();

                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    throw new JsonException("Failed to get property name");
                }

                reader.Read();

                dictionary.Add(propertyName, ExtractValue(ref reader, options));
            }

            return new ConfigSection(dictionary);
        }

        public override void Write(Utf8JsonWriter writer, ConfigSection value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Dictionary, options);
        }

        private object ExtractValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.False:
                    return false;
                
                case JsonTokenType.True:
                    return true;
                
                case JsonTokenType.Null:
                    return null!;
                
                case JsonTokenType.String:
                    return reader.TryGetDateTime(out DateTime date) ? date : reader.GetString()!;
                
                case JsonTokenType.Number:
                    return reader.TryGetInt32(out int result) ? result : reader.GetDouble();
                
                case JsonTokenType.StartObject:
                    return Read(ref reader, null, options);
                
                case JsonTokenType.StartArray:
                    List<object> list = new();
                    while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
                    {
                        list.Add(ExtractValue(ref reader, options));
                    }
                    return list;
                
                default:
                    throw new JsonException($"'{reader.TokenType}' is not supported");
            }
        }
    }
}