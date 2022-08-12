# LLNETUtils

###### [English](README.md) | Русский

Библиотека для [LiteLoader.NET](https://github.com/LiteLDev/LiteLoader.NET) для более удобной разработки плагинов. `LLNETUtils` предоставляет следующие возможности:
- Сохранение ресурсов в папку с данными вашего плагина
- Чтение и редактирование `YAML`, `JSON` и `Properties` конфигов

## Установка

1. Скачайте последний файл `LLNETUtils.dll` из [Releases](https://github.com/S3v3Nice/LLNETUtils/releases)
2. Скопируйте `LLNETUtils.dll` в `plugins/lib` в директории сервера.

## Начало работы

1. **[Необязательно]** Скопируйте документационный файл `LLNETUtils.xml` (из [Releases](https://github.com/S3v3Nice/LLNETUtils/releases)) в папку с `LLNETUtils.dll`
2. Добавьте в своем проекте ссылку на `LLNETUtils.dll`
3. Сделайте так, чтобы ваш основной класс плагина наследовал класс `PluginBase`:

```csharp
[PluginMain("Название плагина")]
public class Main : PluginBase
{
    ...
}
```

## Ресурсы плагина

> Ваши файлы ресурсов должны быть обозначены в проекте как вложенные ресурсы (`EmbeddedResource`).

### Сохранение ресурса

Используйте методы `SaveResource` и `SaveDefaultConfig` класса `PluginBase` для сохранения файла ресурсов в папку с данными плагина.

```csharp
// Допустим, в проекте есть папка "Resources" со следующей структурой:
// - Resources
//     - lang
//         - ru_RU.json
//     - config.yml

// Сохранить (если не существует) "ru_RU.json" в ".../plugins/<плагин>/ru_RU.json"
SaveResource("ru_RU.json")
// Сохранить (с заменой) "ru_RU.json" в ".../plugins/<плагин>/lang/ru_RU.json"
SaveResource("lang/ru_RU.json", true)
// Сохранить (если не существует) "ru_RU.json" в ".../plugins/<плагин>/rus.json"
SaveResource("Resources/lang/ru_RU.json", false, "rus.json")

// Сохранить (если не существует) "config.yml" в ".../plugins/<плагин>/config.yml"
SaveDefaultConfig()
// Сохранить (с заменой) "config.yml" в ".../plugins/<плагин>/config.yml"
SaveDefaultConfig(true)
```

### Получить поток файла ресурсов

Используйте метод `GetResource` класса `PluginBase`. Это позволит вам читать файл ресурсов (используя его поток), не сохраняя его в папку с данными плагина.

```csharp
Stream? resource = GetResource("lang.json");
```

## Конфиги

Класс `Config` позволит вам удобно читать и редактировать `YAML`, `JSON` и `Properties` конфиги.

### Загрузка конфига

Вы можете загружать конфиг из файла или из потока, используя методы `Load` и `Reload` класса `Config`, либо сразу при инициализации.

```csharp
var config = new Config();

// Загрузить из файла
config.Load(Path.Join(DataPath, "ru_RU.json"));
// Загрузить из потока
config.Load(GetResource("ru_RU.json")!, ConfigType.Json);
// Перезагрузить из файла
config.Reload();

// Создать объект и сразу загрузить из файла
var config2 = new Config(Path.Join(DataPath, "ru_RU.json"));
```

Если вы работаете с `config.yml`, вы можете не создавать вручную новый объект `Config` и загружать его. 
Просто используйте свойство `Config` класса `PluginBase`. 
Конфиг загрузится автоматически, если имеется соответствующий файл или вложенный ресурс.

```csharp
Config defaultConfig = Config;
```

### Сохранение конфига

Чтобы сохранить конфиг, используйте метод `Save` класса `Config`.

```csharp
// Сохранить в ".../plugins/<плагин>/lang/ru_RU.json"
config.Save(Path.Join(DataPath, "lang/ru_RU.json"));
```

### Чтение и редактирование конфига

Все нужные методы хранятся в интерфейсе `IConfigSection` (классы `Config` и `ConfigSection` реализуют данный интерфейс).

Пример работы с конфигом:

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
