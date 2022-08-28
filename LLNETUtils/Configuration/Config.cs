using System.Collections;
using System.Diagnostics.CodeAnalysis;
using LLNET.Logger;
using LLNETUtils.Configuration.Serialization;

namespace LLNETUtils.Configuration;

/// <summary>Configuration format type.</summary>
public enum ConfigType
{
    /// <summary>Undefined format type. The format type will be automatically
    /// determined by the file extension, or defined as Yaml.</summary>
    Undefined,
    /// <summary>YAML format type.</summary>
    Yaml,
    /// <summary>JSON format type.</summary>
    Json,
    /// <summary>Properties format type.</summary>
    Properties
}

/// <summary>Class for working with the config.</summary>
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

    /**
     * <param name="filePath">Full path to the config file.</param>
     * <param name="type">Configuration format type.</param>
     * <param name="logger">Logger for displaying occurring errors in the console.</param>
     */
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

    /// <summary>Full path to the config file.</summary>
    public string FilePath { get; set; }
    /// <summary>Logger for displaying occurring errors in the console.</summary>
    public Logger? Logger { get; set; }
    /// <summary>Root config section.</summary>
    public IConfigSection Root { get; set; }

    /// <summary>Configuration format type.</summary>
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
    
    ConfigDictionary IConfigSection.Dictionary
    {
        get => Root.Dictionary;
        set => Root.Dictionary = value;
    }

    /**
     * <summary>Reloads the config from a file (by FilePath).</summary>
     * <returns>true if the config was reloaded successfully; otherwise, false.</returns>
     */
    public bool Reload()
    {
        return Load(FilePath, Type);
    }

    /**
     * <summary>Loads the config from a file.</summary>
     * <param name="filePath">Full path to the file to load the config from.</param>
     * <param name="type">Configuration format type.</param>
     * <returns>true if the config was loaded successfully; otherwise, false.</returns>
     */
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

    /**
     * <summary>Loads the config from a stream.</summary>
     * <param name="stream">Stream to load the config from.</param>
     * <param name="type">Configuration format type.</param>
     * <returns>true if the config was loaded successfully; otherwise, false.</returns>
     */
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

    /**
     * <summary>Saves the config to the specified path.</summary>
     * <param name="filePath">Full path to save the config.</param>
     * <returns>true if the config was saved successfully; otherwise, false.</returns>
     */
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

    public void Clear()
    {
        Root.Clear();
    }

    public bool Contains(string key)
    {
        return Root.Contains(key);
    }

    public void Remove(string key)
    {
        Root.Remove(key);
    }

    public void Set(string key, object value)
    {
        Root.Set(key, value);
    }

    public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
    {
        return Root.TryGet(key, out value);
    }

    public object? Get(string key)
    {
        return Root.Get(key);
    }

    public T? Get<T>(string key, T? defaultValue = default)
    {
        return Root.Get(key, defaultValue);
    }

    public string GetString(string key, string defaultValue = "")
    {
        return Root.GetString(key, defaultValue);
    }

    public int GetInt(string key, int defaultValue = default)
    {
        return Root.GetInt(key, defaultValue);
    }

    public float GetFloat(string key, float defaultValue = default)
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

    public IList<object>? GetList(string key, IList<object>? defaultValue = null)
    {
        return Root.GetList(key, defaultValue);
    }

    public IList<T>? GetList<T>(string key, IList<T>? defaultValue = null)
    {
        return Root.GetList(key, defaultValue);
    }

    public IConfigSection? GetSection(string key, IConfigSection? defaultValue = null)
    {
        return Root.GetSection(key, defaultValue);
    }

    bool IConfigSection.Find(string key, out IDictionary<string, object> dict, out string dictKey)
    {
        return Root.Find(key, out dict, out dictKey);
    }
}