using FormatConversion.Enums;
using FormatConversion.Exceptions;

namespace FormatConversion.Conversion.Converters
{
    internal abstract class BaseConverter
    {
        public abstract FormatType Type{ get; }
        protected BaseConverter(ConversionDIContainer conversionService)
        {
            conversionService.Set(Type, this);
        }
        public abstract void TryConvertFromUniFormat(string outFilePath, UniversalDataFormat uniFormat);
        public abstract UniversalDataFormat TryConvertToUniFormat(string inFilePath);
        protected static UniversalDataFormat HandleConversionTo(Func<UniversalDataFormat> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                throw new UniversalDataConversionException($"Conversion to universal data format failed", ex);
            }
        }
        protected static void HandleConversionFrom(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                throw new UniversalDataConversionException($"Conversion from universal data format failed", ex);
            }
        }
    }
}
