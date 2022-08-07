using LLNET.Logger;
using LLNETUtils.Configuration.Serialization;

namespace LLNETUtils.Configuration;

public enum ConfigType
{
    Undefined,
    Yaml,
    Json,
    Properties
}

public class Config
{
    private static readonly Dictionary<string, ConfigType> FormatTypes = new()
    {
        {".yml", ConfigType.Yaml},
        {".yaml", ConfigType.Yaml},
        {".json", ConfigType.Json},
        {".js", ConfigType.Json},
        {".properties", ConfigType.Properties}
    };

    private IConfigSerializer _serializer;
    private ConfigType _type;

    public Config(string filePath = "", ConfigType type = ConfigType.Undefined, Logger? logger = null)
    {
        _serializer = null!;
        FilePath = filePath;
        Type = type;
        Logger = logger;
        Root = new ConfigSection();

        if (!string.IsNullOrEmpty(filePath))
        {
            Load(FilePath, Type);
        }
    }

    public string FilePath { get; set; }
    public Logger? Logger { get; set; }
    public IConfigSection Root { get; set; }

    public ConfigType Type
    {
        get => _type;
        set
        {
            if (_type == value)
            {
                return;
            }

            _type = value;

            switch (_type)
            {
                case ConfigType.Yaml:
                    _serializer = new YamlConfigSerializer();
                    break;
                case ConfigType.Json:
                    _serializer = new JsonConfigSerializer();
                    break;
                case ConfigType.Properties:
                    _serializer = new PropertiesConfigSerializer();
                    break;
            }
        }
    }

    public bool Reload()
    {
        return Load(FilePath, Type);
    }

    public bool Load(string filePath, ConfigType type = ConfigType.Undefined)
    {
        if (!File.Exists(filePath))
        {
            Logger?.Error.WriteLine($"Could not load {Type} config from a file '{filePath}':\nNo such file!");
            return false;
        }

        if (type != ConfigType.Undefined)
        {
            Type = type;
        }
        else if (Type == ConfigType.Undefined)
        {
            Type = FormatTypes.GetValueOrDefault(Path.GetExtension(filePath), ConfigType.Yaml);
        }

        try
        {
            string data = File.ReadAllText(filePath);
            Root = _serializer.Deserialize(data);
        }
        catch (Exception e)
        {
            Logger?.Error.WriteLine($"Could not load {Type} config from a file '{filePath}':\n{e}");
            return false;
        }

        FilePath = filePath;
        return true;
    }

    public bool Load(Stream stream, ConfigType type = ConfigType.Undefined)
    {
        if (type != ConfigType.Undefined)
        {
            Type = type;
        }
        else if (Type == ConfigType.Undefined)
        {
            Type = ConfigType.Yaml;
        }

        try
        {
            using StreamReader streamReader = new(stream);
            string data = streamReader.ReadToEnd();
            Root = _serializer.Deserialize(data);
        }
        catch (Exception e)
        {
            Logger?.Error.WriteLine($"Could not load {Type} config from a stream:\n{e}");
            return false;
        }

        return true;
    }

    public bool Save(string filePath = "")
    {
        if (filePath == "")
        {
            filePath = FilePath;
        }

        if (!File.Exists(filePath))
        {
            try
            {
                FileInfo file = new(filePath);
                DirectoryInfo? dir = file.Directory;

                if (dir != null && !dir.Exists)
                {
                    dir.Create();
                }
            }
            catch (Exception e)
            {
                Logger?.Error.WriteLine($"Could not save {Type} config to '{filePath}':\n{e}");
                return false;
            }
        }

        string data = _serializer.Serialize(Root);
        File.WriteAllText(filePath, data);
        return true;
    }
}