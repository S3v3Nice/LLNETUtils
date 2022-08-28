using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using LLNET.Core;
using LLNET.Logger;
using LLNETUtils.Configuration;

namespace LLNETUtils;

/// <summary>Class that is the base for the main plugin class.</summary>
public abstract class PluginBase : IPluginInitializer
{
    private const string ConfigResourcePath = "config.yml";
    private Config? _config;

    static PluginBase()
    {
        ServerPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName!)!;
    }

    protected PluginBase()
    {
        if (GetType().GetCustomAttributes(typeof(PluginMainAttribute), true)[0] is not PluginMainAttribute attribute)
        {
            string className = GetType().Name;
            throw new Exception($"Attribute 'PluginMain' is missing in class '{className}'");
        }

        Name = attribute.Name;
        Logger = new Logger(Name);
        DataPath = Path.Join(ServerPath, "plugins", Name);
    }

    /// <summary>Full path to the main server directory.</summary>
    public static string ServerPath { get; }
    /// <summary>Full path to the plugin data directory.</summary>
    public string DataPath { get; }
    /// <summary>Name of the plugin specified in the PluginMain attribute.</summary>
    public string Name { get; }
    /// <summary>Logger of the plugin.</summary>
    public Logger Logger { get; }

    /// <summary>The main config of the plugin. The default is config.yml.</summary>
    public Config Config
    {
        get
        {
            if (_config != null)
            {
                return _config;
            }

            _config = new Config
            {
                FilePath = Path.Join(DataPath, ConfigResourcePath), 
                Type = ConfigType.Yaml, 
                Logger = Logger
            };

            if (File.Exists(_config.FilePath))
            {
                _config.Reload();
            }
            else
            {
                Stream? configStream = GetResource(ConfigResourcePath);
                if (configStream != null)
                {
                    _config.Load(configStream);
                }
            }

            return _config;
        }
        set => _config = value;
    }

    /// <summary>A brief description of the plugin.</summary>
    public abstract string Introduction { get; }
    /// <summary>Version of the plugin.</summary>
    public abstract Version Version { get; }
    /// <summary>Metadata, which may include author name, website, etc.</summary>
    public abstract Dictionary<string, string> MetaData { get; }

    /// <summary>Method that is called after loading this plugin.</summary>
    public abstract void OnInitialize();

    /**
     * <summary>Returns EmbeddedResource by file path.</summary>
     * <param name="resourcePath">Approximate path to the embedded resource file (e.g. "lang/en.yml" or "en.yml").</param>
     * <returns>Stream if a resource with a given path exists; otherwise, null.</returns>
     */
    public Stream? GetResource(string resourcePath)
    {
        string approxResourceName = new Regex(@"\\\\|//|[/\\]").Replace(resourcePath, ".");
        Assembly assembly = GetType().Assembly;
        string[] resourceNames = assembly.GetManifestResourceNames();
        string? resourceName =
            resourceNames.FirstOrDefault(x => x == approxResourceName || x.EndsWith('.' + approxResourceName));

        return string.IsNullOrEmpty(resourceName) ? null : assembly.GetManifestResourceStream(resourceName);
    }

    /**
     * <summary>Saves EmbeddedResource by file path.</summary>
     * <param name="resourcePath">Approximate path to the embedded resource file (e.g. "lang/en.yml" or "en.yml").</param>
     * <param name="replace">Whether a file needs to be replaced if it already exists.</param>
     * <param name="outputPath">Path of the output file in plugin data directory.</param>
     * <returns>true if the resource has been saved; otherwise, false.</returns>
     */
    public bool SaveResource(string resourcePath, bool replace = false, string outputPath = "")
    {
        if (outputPath == "")
        {
            outputPath = resourcePath;
        }

        FileInfo output = new(Path.Join(DataPath, outputPath));

        if (output.Exists && !replace)
        {
            return true;
        }

        try
        {
            Stream? resource = GetResource(resourcePath);

            if (resource == null)
            {
                Logger.error.WriteLine($"Could not save resource '{resourcePath}':\nNo such embedded resource in assembly!");
                return false;
            }

            DirectoryInfo? outputFolder = output.Directory;
            if (outputFolder != null && !outputFolder.Exists)
            {
                outputFolder.Create();
            }

            using FileStream fileStream = output.OpenWrite();
            resource.Seek(0, SeekOrigin.Begin);
            fileStream.SetLength(0);
            resource.CopyTo(fileStream);

            return true;
        }
        catch (Exception e)
        {
            Logger.error.WriteLine($"Could not save resource '{resourcePath}':\n{e}");
            return false;
        }
    }

    /// <summary>Reloads the Config stored in the Config property.</summary>
    public void ReloadConfig()
    {
        Config.Reload();
    }

    /// <summary>Saves the Config stored in the Config property.</summary>
    public void SaveConfig()
    {
        Config.Save();
    }

    /**
     * <summary>Saves the config resource file (config.yml) to the plugin data folder.</summary>
     * <param name="replace">Whether a file needs to be replaced if it already exists.</param>
     */
    public void SaveDefaultConfig(bool replace = false)
    {
        SaveResource(ConfigResourcePath, replace);
    }
}