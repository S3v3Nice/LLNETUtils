using System.Collections;
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

public class Config : IConfigSection
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

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return Root.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public bool ContainsKey(string key)
    {
        return Root.ContainsKey(key);
    }

    public object? Get(string key)
    {
        return Root.Get(key);
    }

    public T? Get<T>(string key, T? defaultValue = default)
    {
        return Root.Get(key, defaultValue);
    }

    public void Set(string key, object value)
    {
        Root.Set(key, value);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return Root.GetString(key, defaultValue);
    }

    public int GetInt(string key, int defaultValue = default)
    {
        return Root.GetInt(key, defaultValue);
    }

    public double GetFloat(string key, float defaultValue = default)
    {
        return Root.GetFloat(key, defaultValue);
    }

    public double GetDouble(string key, double defaultValue = default)
    {
        return Root.GetDouble(key, defaultValue);
    }

    public bool GetBool(string key, bool defaultValue = default)
    {
        return Root.GetBool(key, defaultValue);
    }

    public DateTime GetDateTime(string key, DateTime defaultValue = default)
    {
        return Root.GetDateTime(key, defaultValue);
    }

    public List<object>? GetList(string key, List<object>? defaultValue = null)
    {
        return Root.GetList(key, defaultValue);
    }

    public List<T>? GetList<T>(string key, List<T>? defaultValue = null)
    {
        return Root.GetList(key, defaultValue);
    }

    public IConfigSection? GetSection(string key, IConfigSection? defaultValue = null)
    {
        return Root.GetSection(key, defaultValue);
    }
}
