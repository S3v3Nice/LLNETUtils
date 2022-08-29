using System.Globalization;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.EventEmitters;
using YamlDotNet.Serialization.Utilities;

namespace LLNETUtils.Configuration.Serialization;

internal class YamlConfigSerializer : IConfigSerializer
{
    public ConfigSection Deserialize(string data)
    {
        ConfigSectionConverter converter = new();
        DeserializerBuilder builder = new DeserializerBuilder()
            .WithTypeConverter(converter)
            .WithNodeTypeResolver(new ConfigNodeTypeResolver());
        converter.ValueDeserializer = builder.BuildValueDeserializer();

        ConfigSection? result = builder.Build().Deserialize<ConfigSection?>(data);

        if (result == null)
        {
            throw new ArgumentException("The data provided is not a valid Yaml object!");
        }

        return result;
    }

    public string Serialize(ConfigSection section)
    {
        ConfigSectionConverter converter = new();
        SerializerBuilder builder = new SerializerBuilder()
            .WithTypeConverter(converter)
            .WithEventEmitter(emitter => new ForceQuoteEventEmitter(emitter));
        converter.ValueSerializer = builder.BuildValueSerializer();

        return builder.Build().Serialize(section);
    }
    
    private class ConfigSectionConverter : IYamlTypeConverter
    {
        public IValueSerializer ValueSerializer { get; set; } = null!;
        public IValueDeserializer ValueDeserializer { get; set; } = null!;

        public bool Accepts(Type type)
        {
            return type.IsAssignableTo(typeof(ConfigSection));
        }

        public object ReadYaml(IParser parser, Type type)
        {
            ConfigDictionary dict = (ConfigDictionary) ValueDeserializer.DeserializeValue(parser,
                typeof(ConfigDictionary), new SerializerState(), ValueDeserializer)!;

            return new ConfigSection(dict);
        }

        public void WriteYaml(IEmitter emitter, object? value, Type type)
        {
            ConfigSection section = (ConfigSection) value!;
            ValueSerializer.SerializeValue(emitter, section.Dictionary, typeof(IDictionary<string, object>));
        }
    }

    private class ForceQuoteEventEmitter : ChainedEventEmitter
    {
        public ForceQuoteEventEmitter(IEventEmitter nextEmitter) : base(nextEmitter)
        {
        }

        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.Type == typeof(string) && eventInfo.Source.StaticType == typeof(object))
            {
                eventInfo.Style = ScalarStyle.DoubleQuoted;
            }

            base.Emit(eventInfo, emitter);
        }
    }

    private class ConfigNodeTypeResolver : INodeTypeResolver
    {
        bool INodeTypeResolver.Resolve(NodeEvent? nodeEvent, ref Type currentType)
        {
            if (currentType != typeof(object))
            {
                return false;
            }

            if (nodeEvent is MappingStart)
            {
                currentType = typeof(ConfigSection);
                return true;
            }

            if (nodeEvent is Scalar scalar)
            {
                if (scalar.IsQuotedImplicit)
                {
                    currentType = typeof(string);
                    return true;
                }

                if (bool.TryParse(scalar.Value, out bool _))
                {
                    currentType = typeof(bool);
                    return true;
                }

                if (int.TryParse(scalar.Value, out int _))
                {
                    currentType = typeof(int);
                    return true;
                }

                if (double.TryParse(scalar.Value, NumberStyles.Float | NumberStyles.AllowThousands,
                        CultureInfo.InvariantCulture, out double _))
                {
                    currentType = typeof(double);
                    return true;
                }

                if (DateTime.TryParse(scalar.Value, out DateTime _))
                {
                    currentType = typeof(DateTime);
                    return true;
                }
            }

            return false;
        }
    }
}