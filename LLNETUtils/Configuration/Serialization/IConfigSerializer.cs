namespace LLNETUtils.Configuration.Serialization;

public interface IConfigSerializer
{
    ConfigSection Deserialize(string data);

    string Serialize(ConfigSection section);
}