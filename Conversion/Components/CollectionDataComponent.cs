using FormatConversion.Enums;

namespace FormatConversion.Conversion.Components
{
    internal class CollectionDataComponent() : IComplexDataComponent
    {
        public DataComponentType Type => DataComponentType.Collection;
        public List<IDataComponent> Items { get; } = [];
        public IEnumerable<IDataComponent> GetChildren()
        {
            return Items;
        }
    }
}
