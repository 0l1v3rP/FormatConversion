using FormatConversion.Conversion.Components;
using FormatConversion.Enums;

namespace FormatConversion.Conversion.Converters
{
    internal class CsvConverter(ConversionDIContainer conversionService) : BaseConverter(conversionService)
    {
        public override FormatType Type => FormatType.CSV;

        #region From
        public override void TryConvertFromUniFormat(string inFilePath, UniversalDataFormat uniFormat)
        {

            HandleConversionFrom(() => {
                throw new NotImplementedException();
            });
        }

     
        #endregion
        #region To
        public override UniversalDataFormat TryConvertToUniFormat(string outFilePath)
        {
            return HandleConversionTo(() =>
            {
                CollectionDataComponent collectionData = new();
                using StreamReader reader = new(outFilePath);
                string? line;
                bool firstLine = true;
                string[] headers = [];
                while ((line = reader.ReadLine()) != null)
                {
                    var values = line.Split(',');

                    if (firstLine)
                    {
                        headers = values;
                        firstLine = false;
                    }
                    else
                    {
                        StructuredDataComponent structuredData = new();

                        foreach (var (value, header) in values.Zip(headers))
                        {
                            var parsedValue = PrimitiveDataComponent.ParseStringValue(value.Trim());
                            structuredData.Fields.Add(header, new PrimitiveDataComponent(parsedValue));
                        }
                        CollectionDataComponent.Items.Add(structuredData);
                    }
                }
                return new UniversalDataFormat(collectionData);
            });
        }
        #endregion
    }
}