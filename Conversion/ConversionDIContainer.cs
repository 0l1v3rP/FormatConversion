using FormatConversion.Conversion.Converters;
using FormatConversion.Enums;
using System.Reflection;

namespace FormatConversion.Conversion
{
    internal class ConversionDIContainer
    {
        private static ConversionDIContainer? _instance;
        public static ConversionDIContainer GetService()
        {
            _instance ??= new ConversionDIContainer();
            return _instance;
        }
        private readonly Dictionary<FormatType, BaseConverter> _converters = [];

        private ConversionDIContainer() {
            var assembly = Assembly.GetExecutingAssembly();

            var converterTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseConverter)) &&
                            t.Namespace == "FormatConversion.Conversion.Converters");

            foreach (var type in converterTypes)
            {
                Activator.CreateInstance(type, this);
            }
        }
        public void Set(FormatType type, BaseConverter converter)
        {
            _converters.Add(type, converter);
        }
        public BaseConverter Get(FormatType type)
        {
            if (_converters.TryGetValue(type, out var converter))
            {
                return converter;
            }
            throw new NotSupportedException($"No converter found for {type}");
        }
    }
}