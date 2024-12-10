namespace FormatConversion.Conversion.Components
{
    internal interface IComplexDataComponent : IDataComponent
    { 
        IEnumerable<IDataComponent> GetChildren();
    }
}