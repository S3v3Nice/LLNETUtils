# LLNETUtils

##### [English](README.md) | Русский

Библиотека для [LiteLoader.NET](https://github.com/LiteLDev/LiteLoader.NET) для более удобной разработки плагинов. Предоставляется следующий функционал:
- Сохранение ресурсов в папку с данными вашего плагина
- Чтение и редактирование `YAML`, `JSON` и `Properties` конфигов

## Начало работы

1. Скачайте последний файл <code>LLNETUtils-<i>версия</i>.zip</code> из [Releases](https://github.com/S3v3Nice/LLNETUtils/releases)
2. Распакуйте содержимое архива в `plugins/lib` в директории сервера
3. Добавьте в своем проекте ссылку на `LLNETUtils.dll`
4. Сделайте так, чтобы ваш основной класс плагина наследовал класс `PluginBase`:

```csharp
[PluginMain("Имя")]
public class Main : PluginBase
{
    ...
}
```

## Ресурсы плагина

> Ваши файлы ресурсов должны быть обозначены в проекте как `EmbeddedResource`.

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

Вы можете загружать конфиг из файла или из потока, используя метод `Load` класса `Config`.

```csharp
var config = new Config();

// Загрузить из файла
config.Load(Path.Join(DataPath, "ru_RU.json"));
// Загрузить из потока
config.Load(GetResource("ru_RU.json")!, ConfigType.Json);
```

> Если вы работаете с `config.yml`, вы можете не создавать объект `Config` и потом загружать его, просто используйте свойство `Config` класса `PluginBase`.

### Сохранение конфига

Чтобы сохранить конфиг, используйте метод `Save` класса `Config`.

```csharp
// Сохранить в ".../plugins/<плагин>/lang/ru_RU.json"
config.Save(Path.Join(DataPath, "lang/ru_RU.json"));
```

### Чтение и редактирование конфига

Используйте свойство `Root` класса `Config`, чтобы получить корневой раздел (объект `ConfigSection`), который позволит вам читать и редактировать конфиг.

```csharp
ConfigSection root = config.Root;

// Чтение конфига
object? unknown         = root.Get("unknown");
string? name            = root.Get<string>("name");
int age                 = root.GetInt("age");
DateTime birthDate      = root.GetDateTime("birthDate");
List<string>? languages = root.GetList<string>("languages");
ConfigSection settings  = root.GetSection("settings");

// Редактирование конфига
root.Set("name", "Pavel");
root.Set("hobby", new [] {"Coding", "Reading", "Playing"});
root.Set("isAdmin", true);
```