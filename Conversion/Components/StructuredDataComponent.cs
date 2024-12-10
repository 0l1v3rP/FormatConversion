using FormatConversion.Enums;

namespace FormatConversion.Conversion.Components
{
    internal class StructuredDataComponent() : IComplexDataComponent
    {
        public DataComponentType Type => DataComponentType.Structure;

        public Dictionary<string, IDataComponent> Fields { get; } = [];

        public IEnumerable<IDataComponent> GetChildren()
        {
            return Fields.Values;
        }
    }
}