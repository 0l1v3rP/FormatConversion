using FormatConversion.Conversion;
using FormatConversion.Conversion.Commands;
using FormatConversion.Enums;
namespace FormatConversion
{
    public class ConversionService
    {
        private readonly ConversionDIContainer _conversionDI;
        public ConversionService()
        {
            _conversionDI = ConversionDIContainer.GetService();
        }
        public void Convert(FormatType inFormat, FormatType outFormat, string inFilePath, string outFilePath, params CommandType[] commands)
        {
            var uniFormat = _conversionDI.Get(inFormat).TryConvertToUniFormat(inFilePath);
            var iter = new DataComponentIterator(uniFormat.Root);
            foreach (var commandType in commands)
            {
                var command =  CommandFactory.GetCommand(commandType);
                iter.ExecuteCommand(command);
            }
            _conversionDI.Get(outFormat).TryConvertFromUniFormat(outFilePath, uniFormat);
        }
    }
}