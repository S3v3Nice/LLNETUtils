using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LLNETUtils.Utils;

namespace LLNETUtils.Configuration.Serialization;

internal class JsonConfigSerializer : IConfigSerializer
{
    public IConfigSection Deserialize(string data)
    {
        JsonSerializerOptions options = new()
        {
            Converters = {new ConfigSectionConverterFactory()}
        };
        IConfigSection? result = JsonSerializer.Deserialize<ConfigSection>(data, options);

        if (result == null)
        {
            throw new ArgumentException("The data provided is not a valid Json object!");
        }

        return result;
    }

    public string Serialize(IConfigSection section)
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            Converters = {new ConfigSectionConverterFactory()}
        };

        return JsonSerializer.Serialize(section, options);
    }

    private class ConfigSectionConverterFactory : JsonConverterFactory
    {
        private static readonly JsonConverter Converter = new ConfigSectionConverter();

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsAssignableTo(typeof(IConfigSection));
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Converter;
        }
    }

    private class ConfigSectionConverter : JsonConverter<IConfigSection>
    {
        public override IConfigSection? Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            var dictionary = JsonSerializer.Deserialize<LinkedDictionary<string, object>>(ref reader, options);
            IConfigSection section = new ConfigSection();
            section.Dictionary = dictionary!;

            return section;
        }

        public override void Write(Utf8JsonWriter writer, IConfigSection value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.Dictionary, options);
        }
    }
}