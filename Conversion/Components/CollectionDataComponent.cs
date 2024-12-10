using FormatConversion.Enums;

namespace FormatConversion.Conversion.Components
{
    internal class CollectionDataComponent() : IComplexDataComponent
    {
        public DataComponentType Type => DataComponentType.Collection;
        public static List<IDataComponent> Items => [];
        public IEnumerable<IDataComponent> GetChildren()
        {
            return Items;
        }
    }
}
