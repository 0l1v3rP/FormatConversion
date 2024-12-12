using FormatConversion.Enums;
namespace FormatConversion.Conversion.Components
{
    internal class PrimitiveDataComponent(object? value) : IDataComponent
    {
        public DataComponentType Type => DataComponentType.Primitive;

        public object? Value { get; set; } = value;
        public Type ValueType { get; } = value?.GetType() ?? typeof(object);
        public override string ToString() => Value?.ToString() ?? "null";

        public static object? ParseStringValue(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            if (bool.TryParse(value, out bool boolResult))
                return boolResult;

            if (int.TryParse(value, out int intResult))
                return intResult;

            if (long.TryParse(value, out long longResult))
                return longResult;

            if (decimal.TryParse(value, out decimal decimalResult))
                return decimalResult;

            if (double.TryParse(value, out double doubleResult))
                return doubleResult;

            if (DateTime.TryParse(value, out DateTime dateTimeResult))
                return dateTimeResult;

            return value;
        }
    }
}
