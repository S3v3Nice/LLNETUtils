using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using LLNET.Core;
using LLNET.Logger;
using LLNETUtils.Configuration;

namespace LLNETUtils;

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

    public static string ServerPath { get; }
    public string DataPath { get; }
    public string Name { get; }
    public Logger Logger { get; }

    public Config Config
    {
        get
        {
            if (_config != null)
            {
                return _config;
            }

            _config = new Config(Path.Join(DataPath, ConfigResourcePath), ConfigType.Yaml, Logger);

            if (!File.Exists(_config.FilePath))
            {
                Stream? configStream = GetResource(ConfigResourcePath);
                if (configStream != null)
                {
                    _config.Load(configStream);
                }
            }

            return _config;
        }
    }

    public abstract string Introduction { get; }
    public abstract Version Version { get; }
    public abstract Dictionary<string, string> MetaData { get; }

    public abstract void OnInitialize();

    /**
     * <summary>Get EmbeddedResource by file path</summary>
     * <param name="resourcePath">Approximate path to the embedded resource file (e.g. "lang/en.yml" or "en.yml")</param>
     * <returns>Stream - if a resource with a given path exists, null - if a resource doesn't exist</returns>
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
     * <summary>Save EmbeddedResource by file path</summary>
     * <param name="resourcePath">Approximate path to the embedded resource file (e.g. "lang/ru.yml" or "ru.yml")</param>
     * <param name="replace">Is it necessary to replace the file if it already exists</param>
     * <param name="outputPath">Path of the output file in plugin data directory</param>
     * <returns>true if the resource has been saved, false - if has not</returns>
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

    /**
     * <summary>Load config resource file (config.yml) from plugin data folder</summary>
     */
    public void ReloadConfig()
    {
        Config.Reload();
    }

    /**
     * <summary>
     *     Save config to plugin data folder. This includes changes that have been made
     *     to the config at runtime (using the Config class)
     * </summary>
     */
    public void SaveConfig()
    {
        Config.Save();
    }

    /**
     * <summary>
     *     Save config EmbeddedResource file (config.yml) to plugin data folder. This does NOT include config changes
     *     that have been made at runtime (using the Config class)
     * </summary>
     * <param name="replace">Is it necessary to replace an existing file</param>
     */
    public void SaveDefaultConfig(bool replace = false)
    {
        SaveResource(ConfigResourcePath, replace);
    }
}