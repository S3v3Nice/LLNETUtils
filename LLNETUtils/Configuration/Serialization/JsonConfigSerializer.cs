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
        ConfigDictionary? result = JsonSerializer.Deserialize<ConfigDictionary>(data);

        if (result == null)
        {
            throw new ArgumentException("The data provided is not a valid Json object!");
        }

        return new ConfigSection(result);
    }

    public string Serialize(IConfigSection section)
    {
        JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        return JsonSerializer.Serialize(section.Dictionary, options);
    }
}