using LLNET.Logger;
using LLNETUtils.Configuration.Serialization;

namespace LLNETUtils.Configuration;

public class Config
{
    public enum ConfigType
    {
        Undefined,
        Yaml,
        Json,
        Properties
    }

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

        Load(FilePath, Type);
    }

    public string FilePath { get; set; }
    public Logger? Logger { get; set; }
    public ConfigSection Root { get; private set; }

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
                case ConfigType.Undefined:
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
            return false;
        }

        if (type != ConfigType.Undefined)
        {
            Type = type;
        }
        else
        {
            if (Type == ConfigType.Undefined)
            {
                FormatTypes.TryGetValue(Path.GetExtension(filePath), out type);
                Type = type;
            }
        }

        try
        {
            string data = File.ReadAllText(filePath);
            Root = _serializer.Deserialize(data);
        }
        catch (Exception e)
        {
            Logger?.Error.WriteLine(e);
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

        try
        {
            using StreamReader streamReader = new(stream);
            string data = streamReader.ReadToEnd();
            Root = _serializer.Deserialize(data);
        }
        catch (Exception e)
        {
            Logger?.Error.WriteLine(e);
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
                Logger?.Error.WriteLine(e);
                return false;
            }
        }

        string data = _serializer.Serialize(Root);
        File.WriteAllText(filePath, data);
        return true;
    }
}