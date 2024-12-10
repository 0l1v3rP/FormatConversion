using FormatConversion.Conversion.Components;
using FormatConversion.Enums;
using System.Text;
using System.Text.Json;

namespace FormatConversion.Conversion.Converters
{
    internal class JsonConverter(ConversionDIContainer conversionService) : BaseConverter(conversionService)
    {
        public override FormatType Type => FormatType.JSON;
        #region From
        public override void TryConvertFromUniFormat(string outFilePath, UniversalDataFormat uniFormat)
        {
            HandleConversionFrom(() => {
                var writerOptions = new JsonWriterOptions
                {
                    Indented = true
                };
                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, writerOptions))
                {
                    WriteDataComponent(writer, uniFormat.Root);
                }

                string jsonContent = Encoding.UTF8.GetString(stream.ToArray());

                File.WriteAllText(outFilePath, jsonContent);
            });
        }

        private void WriteDataComponent(Utf8JsonWriter writer, IDataComponent component)
        {
            switch (component.Type)
            {
                case DataComponentType.Primitive:
                    WritePrimitive(writer, (PrimitiveDataComponent)component);
                    break;
                case DataComponentType.Structure:
                    WriteStructure(writer, (StructuredDataComponent)component);
                    break;
                case DataComponentType.Collection:
                    WriteCollection(writer, (CollectionDataComponent)component);
                    break;
                default:
                    throw new InvalidOperationException("Unsupported data component type.");
            }
        }

        private void WritePrimitive(Utf8JsonWriter writer, PrimitiveDataComponent primitive)
        {
            switch (primitive.Value)
            {
                case string stringValue:
                    writer.WriteStringValue(stringValue);
                    break;
                case int intValue:
                    writer.WriteNumberValue(intValue);
                    break;
                case long longValue:
                    writer.WriteNumberValue(longValue);
                    break;
                case decimal decimalValue:
                    writer.WriteNumberValue(decimalValue);
                    break;
                case double doubleValue:
                    writer.WriteNumberValue(doubleValue);
                    break;
                case float floatValue:
                    writer.WriteNumberValue(floatValue);
                    break;
                case bool boolValue:
                    writer.WriteBooleanValue(boolValue);
                    break;
                case null:
                    writer.WriteNullValue();
                    break;
                default:
                    writer.WriteStringValue(primitive.Value?.ToString());
                    break;
            }
        }

        private void WriteStructure(Utf8JsonWriter writer, StructuredDataComponent structure)
        {
            writer.WriteStartObject();

            foreach (var field in structure.Fields)
            {
                writer.WritePropertyName(field.Key);
                WriteDataComponent(writer, field.Value);
            }

            writer.WriteEndObject();
        }

        private void WriteCollection(Utf8JsonWriter writer, CollectionDataComponent collection)
        {
            writer.WriteStartArray();

            foreach (var item in CollectionDataComponent.Items)
            {
                WriteDataComponent(writer, item);
            }

            writer.WriteEndArray();
        }

        #endregion
        #region To

        public override UniversalDataFormat TryConvertToUniFormat(string inFilePath)
        {
            return HandleConversionTo(() => {
                string json = File.ReadAllText(inFilePath);
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);
                var root = ParseDataComponent(jsonElement);
                return new UniversalDataFormat(root);
            });
        }

        private IDataComponent ParseDataComponent(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => ParseStructure(element),
                JsonValueKind.Array => ParseCollection(element),
                JsonValueKind.String or JsonValueKind.Number or JsonValueKind.True or JsonValueKind.False => ParsePrimitive(element),
                _ => throw new InvalidOperationException("Unsupported JSON element type."),
            };
        }

        private StructuredDataComponent ParseStructure(JsonElement element)
        {
            var structure = new StructuredDataComponent();
            foreach (var property in element.EnumerateObject())
            {
                var fieldName = property.Name;
                var fieldValue = property.Value;
                structure.Fields.Add(fieldName, ParseDataComponent(fieldValue));
            }
            return structure;
        }

        private CollectionDataComponent ParseCollection(JsonElement element)
        {
            var collection = new CollectionDataComponent();
            foreach (var item in element.EnumerateArray())
            {
                CollectionDataComponent.Items.Add(ParseDataComponent(item));
            }
            return collection;
        }

        private static PrimitiveDataComponent ParsePrimitive(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => new PrimitiveDataComponent(element.GetString()!),
                JsonValueKind.Number => new PrimitiveDataComponent(element.GetDecimal()),
                JsonValueKind.True => new PrimitiveDataComponent(true),
                JsonValueKind.False => new PrimitiveDataComponent(false),
                _ => throw new InvalidOperationException("Unsupported primitive type.")
            };
        }
        #endregion
    }
}