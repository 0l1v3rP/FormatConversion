using FormatConversion.Conversion.Components;
using FormatConversion.Enums;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace FormatConversion.Conversion.Converters
{
    internal class YamlConverter(ConversionDIContainer conversionService) : BaseConverter(conversionService)
    {
        public override FormatType Type => FormatType.YAML;

        #region From
        public override void TryConvertFromUniFormat(string filePath, UniversalDataFormat uniFormat)
        {
            HandleConversionFrom(() => {
                var yamlObject = ConvertDataComponentToObject(uniFormat.Root);

                var serializer = new SerializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                string yamlContent = serializer.Serialize(yamlObject);

                File.WriteAllText(filePath, yamlContent);
            });
        }

        private object? ConvertDataComponentToObject(IDataComponent component)
        {

            return component.Type switch
            {
                DataComponentType.Primitive =>
                    (component as PrimitiveDataComponent)?.Value,

                DataComponentType.Structure =>
                    ConvertStructureToObject((StructuredDataComponent)component),

                DataComponentType.Collection =>
                    ConvertCollectionToObject((CollectionDataComponent)component),

                _ => throw new InvalidOperationException("Unsupported data component type")
            };
        }

        private Dictionary<string, object?> ConvertStructureToObject(StructuredDataComponent structure)
        {
            return structure.Fields
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => ConvertDataComponentToObject(kvp.Value)
                );
        }

        private List<object?> ConvertCollectionToObject(CollectionDataComponent collection)
        {
            return CollectionDataComponent.Items
                .Select(ConvertDataComponentToObject)
                .ToList();
        }
        #endregion
        #region To
        public override UniversalDataFormat TryConvertToUniFormat(string inFilePath)
        {
            return HandleConversionTo(() =>
            {
                var yamlContent = File.ReadAllText(inFilePath);
                var deserializer = new DeserializerBuilder().Build();
                var yamlObject = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);
                var rootComponent = ParseYamlToDataComponent(yamlObject);
                return new UniversalDataFormat(rootComponent);
            });
        }

        private IDataComponent ParseYamlToDataComponent(object value)
        {
            return value switch
            {
                IDictionary<object, object> dict => ParseStructure(dict),
                IEnumerable<object> list => ParseCollection(list),
                _ => ParsePrimitive(value),
            };
        }

        private StructuredDataComponent ParseStructure(IDictionary<object, object> dictionary)
        {
            var structure = new StructuredDataComponent();
            foreach (var kvp in dictionary)
            {
                structure.Fields.Add(kvp.Key.ToString()!, ParseYamlToDataComponent(kvp.Value));
            }
            return structure;
        }

        private CollectionDataComponent ParseCollection(IEnumerable<object> list)
        {
            var collection = new CollectionDataComponent();
            foreach (var item in list)
            {
                CollectionDataComponent.Items.Add(ParseYamlToDataComponent(item));
            }
            return collection;
        }

        private static PrimitiveDataComponent ParsePrimitive(object value)
        {
            return new PrimitiveDataComponent(value);
        }
        #endregion
    }
}