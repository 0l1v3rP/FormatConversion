using FormatConversion.Conversion.Components;
using FormatConversion.Enums;
using System;

namespace FormatConversion.Conversion.Converters
{
    internal class CsvConverter(ConversionDIContainer conversionService) : BaseConverter(conversionService)
    {
        public override FormatType Type => FormatType.CSV;

        #region From
        public override void TryConvertFromUniFormat(string inFilePath, UniversalDataFormat uniFormat)
        {
            HandleConversionFrom(() =>
            {
                HashSet<string> columnNames = [];
                List<Dictionary<string, object?>> dataRows = [];
              
                CollectColumnNames(uniFormat.Root, "", columnNames);
                CollectDataRows(uniFormat.Root, columnNames, dataRows);
                using StreamWriter writer = new(inFilePath);
                writer.WriteLine(string.Join(",", columnNames));
                foreach (var row in dataRows)
                {
                    var rowValues = columnNames.Select(col =>
                        row.TryGetValue(col, out var value) ? value?.ToString() ?? "" : ""
                    );
                    writer.WriteLine(string.Join(",", rowValues));
                }
            });
        }
          
    private void CollectColumnNames(IDataComponent component, string prefix, HashSet<string> columnNames)
    {
            if (component.Type.Equals(DataComponentType.Collection))
            {
                var collection = (CollectionDataComponent)component;
                foreach (var item in collection.Items)
                {
                    CollectColumnNamesSanitized(item, prefix, columnNames);
                }
            }
            else
            {
                CollectColumnNamesSanitized(component, prefix, columnNames);
            }
        }

    private void CollectColumnNamesSanitized(IDataComponent component, string prefix, HashSet<string> columnNames)
        {
            switch (component)
            {
                case PrimitiveDataComponent primitiveComponent:
                    columnNames.Add(prefix);
                    break;

                case StructuredDataComponent structuredComponent:
                    foreach (var field in structuredComponent.Fields)
                    {
                    CollectColumnNamesSanitized(
                            field.Value,
                            string.IsNullOrEmpty(prefix)
                                ? field.Key
                                : $"{prefix}.{field.Key}",
                            columnNames
                        );
                    }
                    break;

                case CollectionDataComponent collectionComponent:
                    for (int i = 0; i < collectionComponent.Items.Count; i++)
                    {
                        CollectColumnNamesSanitized(
                            collectionComponent.Items[i],
                            prefix + "[" + i + "]",
                            columnNames
                        );
                    }
                    break;
            }
        }

        private void CollectDataRows(IDataComponent component, HashSet<string> columnNames, List<Dictionary<string, object?>> dataRows)
        {
            var rows = new Dictionary<string, object?>();

            switch (component)
            {
                case PrimitiveDataComponent primitiveComponent:
                    rows.Add("", primitiveComponent.Value);
                    dataRows.Add(rows);
                break;

                case StructuredDataComponent structuredComponent:
                    foreach (var field in structuredComponent.Fields)
                    {
                        TraverseAndAddToRow(field.Value, field.Key, rows);
                    }
                    dataRows.Add(rows);
                    break;

                case CollectionDataComponent collectionComponent:
                    for(int i = 0; i < collectionComponent.Items.Count; ++i)
                    {
                        TraverseAndAddToRow(collectionComponent.Items[i], "", rows);
                        dataRows.Add(rows);
                    }
                    break;
            }
        }

        private void TraverseAndAddToRow(IDataComponent component, string prefix, Dictionary<string, object?> row)
        {
            switch (component)
            {
                case PrimitiveDataComponent primitiveComponent:
                    row[prefix] = primitiveComponent.Value;
                    break;

                case StructuredDataComponent structuredComponent:
                    foreach (var field in structuredComponent.Fields)
                    {
                        TraverseAndAddToRow(
                            field.Value,
                            string.IsNullOrEmpty(prefix)
                                ? field.Key
                                : $"{prefix}.{field.Key}",
                            row
                        );
                    }
                    break;

                case CollectionDataComponent collectionComponent:
                    for (int i = 0; i < collectionComponent.Items.Count; i++)
                    {
                        var normalizedPrefix = prefix == "" ? "" : prefix + "[" + i + "]";
                        TraverseAndAddToRow(
                           collectionComponent.Items[i],
                           normalizedPrefix,
                           row
                       );
                    }
                    break;
            }
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
                        collectionData.Items.Add(structuredData);
                    }
                }
                return new UniversalDataFormat(collectionData);
            });
        }
        #endregion
    }
}