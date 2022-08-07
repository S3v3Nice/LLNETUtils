namespace LLNETUtils.Configuration.Serialization;

internal interface IConfigSerializer
{
    IConfigSection Deserialize(string data);

    string Serialize(IConfigSection section);
}