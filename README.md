# LLNETUtils

##### English | [Русский](README_ru.md)

Library for [LiteLoader.NET](https://github.com/LiteLDev/LiteLoader.NET) for easier plugin development. It provides features such as:
- Saving plugin resources to the plugin data folder
- Reading and editing `YAML`, `JSON` and `Properties` configs

## Getting started
1. Download the latest <code>LLNETUtils-<i>version</i>.zip</code> from [Releases](https://github.com/S3v3Nice/LLNETUtils/releases)
2. Unzip all files to `plugins/lib` in the server directory
3. Add `LLNETUtils.dll` to the project references
4. Make your main plugin class inherit the `PluginBase` class:

```csharp
[PluginMain("Name")]
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

You can load the config from a file or from a stream using the `Load` method of the `Config` class.

```csharp
var config = new Config();

// Load from a file
config.Load(Path.Join(DataPath, "en_US.json"));
// Load from a stream
config.Load(GetResource("en_US.json")!, ConfigType.Json);
```

> If you are working with `config.yml`, you don't have to create an instance of the `Config` class and load it, just use the `Config` property of the `PluginBase` class.

### Save config

To save the config, use the `Save` method of the `Config` class.

```csharp
// Save to ".../plugins/<plugin>/lang/en_US.json"
config.Save(Path.Join(DataPath, "lang/en_US.json"));
```

### Read and modify config

Use the `Root` property of the `Config` class to get the root `ConfigSection`, which allows you to read and modify the config.

```csharp
ConfigSection root = config.Root;

// Read config
object? unknown         = root.Get("unknown");
string? name            = root.Get<string>("name");
int age                 = root.GetInt("age");
DateTime birthDate      = root.GetDateTime("birthDate");
List<string>? languages = root.GetList<string>("languages");
ConfigSection settings  = root.GetSection("settings");

// Modify config
root.Set("name", "Pavel");
root.Set("hobby", new [] {"Coding", "Reading", "Playing"});
root.Set("isAdmin", true);
```