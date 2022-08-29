# LLNETUtils

###### English | [Русский](README_ru.md)

Library for [LiteLoader.NET](https://github.com/LiteLDev/LiteLoader.NET) for easier plugin development. It provides features such as:
- Saving plugin resources to the plugin data folder
- Reading and editing `YAML`, `JSON` and `Properties` configs

## Install

1. Download the latest `LLNETUtils.dll` from [Releases](https://github.com/S3v3Nice/LLNETUtils/releases)
2. Copy `LLNETUtils.dll` into `plugins/lib` in the server directory

## Get started

To start working with the library, you can add the NuGet package [LLNETUtils](https://www.nuget.org/packages/LLNETUtils/) to your project. Or you can do the following:

1. **[Optional]** Copy the documentation file `LLNETUtils.xml` (from [Releases](https://github.com/S3v3Nice/LLNETUtils/releases)) to the folder with `LLNETUtils.dll`
2. Add `LLNETUtils.dll` to the project references

Next make your main plugin class inherit the `PluginBase` class:

```csharp
[PluginMain("Plugin name")]
public class Main : PluginBase
{
    ...
}
```

## Plugin resources
> Your resource files should be marked as `EmbeddedResource` in the project.

### Save resource file
Use the `SaveResource` and `SaveDefaultConfig` methods of the `PluginBase` class to save the resource file to plugin data folder.

```csharp
// Suppose we have a "Resources" folder in the project with the following structure:
// - Resources
//     - lang
//         - en_US.json
//     - config.yml

// Save (if does not exist) "en_US.json" to ".../plugins/<plugin>/en_US.json"
SaveResource("en_US.json")
// Save (with replace) "en_US.json" to ".../plugins/<plugin>/lang/en_US.json"
SaveResource("lang/en_US.json", true)
// Save (if does not exist) "en_US.json" to ".../plugins/<plugin>/eng.json"
SaveResource("Resources/lang/en_US.json", false, "eng.json")

// Save (if does not exist) "config.yml" to ".../plugins/<plugin>/config.yml"
SaveDefaultConfig()
// Save (with replace) "config.yml" to ".../plugins/<plugin>/config.yml"
SaveDefaultConfig(true)
```

### Get resource file stream
Use the `GetResource` method of the `PluginBase` class. This allows you to read the resource file (using it's stream) without saving it to data folder.

```csharp
Stream? resource = GetResource("lang.json");
```

## Configs

The `Config` class allows you to easily read and modify `YAML`, `JSON` and `Properties` configs.

### Load config

You can load the config from a file or from a stream using the `Load` and `Reload` methods of the `Config` class, or immediately on initialization.

```csharp
var config = new Config();

// Load from a file
config.Load(Path.Join(DataPath, "en_US.json"));
// Load from a stream
config.Load(GetResource("en_US.json")!, ConfigType.Json);
// Reload from a file
config.Reload();

// Create an object and load it immediately from a file
var config2 = new Config(Path.Join(DataPath, "en_US.json"));
```

If you work with `config.yml`, you don't have to manually create a new `Config` object and load it.
Just use the `Config` property of the `PluginBase` class.
The config will load automatically if there is a corresponding file or embedded resource.

```csharp
Config defaultConfig = Config;
```

### Save config

To save the config, use the `Save` method of the `Config` class.

```csharp
// Save to ".../plugins/<plugin>/lang/en_US.json"
config.Save(Path.Join(DataPath, "lang/en_US.json"));
```

### Read and modify config

All necessary methods are in the `IConfigSection` interface (classes `Config` and `ConfigSection` implement this interface).

Example of work with the config:

```csharp
Config config = new Config(Path.Join(DataPath, "user1.yml"));

string? name            = config.Get<string>("name");
string lastName         = config.GetString("last-name", "Unknown");
int id                  = config.GetInt("id");
List<string>? perms     = config.GetList<string>("perms");
IConfigSection settings = config.GetSection("settings")!;
DateTime birthDate      = settings.GetDateTime("birth-date");

config.Set("name", "Pavel");
config.Set("extra-data", new ConfigSection());
config.Remove("perms");
settings.Clear();
config.Set("settings.mute-notifications", true);
```
