namespace LLNETUtils.Configuration.Serialization;

internal interface IConfigSerializer
{
    ConfigSection Deserialize(string data);

    string Serialize(ConfigSection section);
}