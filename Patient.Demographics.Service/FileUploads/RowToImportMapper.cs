using Patient.Demographics.Common;
using Patient.Demographics.Repository.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Patient.Demographics.Common.Csv;
using Patient.Demographics.Data.Entities.BatchProcess;
using Newtonsoft.Json.Linq;

namespace Patient.Demographics.Service.FileUploads
{
    public interface IRowToImportMapper
    {
        Task<IList<RowToImport>> Map(IList<BatchProcessRecordEntity> rows);
    }

    public class RowToImportMapper : IRowToImportMapper
    {
        public RowToImportMapper()
        {
        }

        public async Task<IList<RowToImport>> Map(IList<BatchProcessRecordEntity> rows)
        {
            var dtos = new List<RowToImport>();

            string[] sourceColumnHeaders = GetSourceColumnHeaders(rows).Select((h => h.ToLower())).ToArray();

            List<BatchProcessRecordEntity> dataRowDtos = rows.OrderBy(row => row.RowNumber)
                                                    .Where(row => row.Status == BatchProcessRecordStatuses.Ok.Name)
                                                    .ToList();

            foreach (var row in dataRowDtos)
            {
                var parsedRow = JArray.Parse(row.Row).ToObject<string[]>();
                var productDto = new RowToImport { UploadId = row.BatchProcessId, RowNumber = row.RowNumber, ParsedRow = parsedRow };
                dtos.Add(productDto);
            }

            return dtos;
        }


        private IList<string> GetSourceColumnHeaders(IList<BatchProcessRecordEntity> rows)
        {
            return ParseDataRowDto(rows.Single(r => r.Status == BatchProcessRecordStatuses.Header.Name));
        }

        //private int GetIndexForColumn(string name, string[] sourceColumnHeaders, IList<LiteralColumnMappingDto> columnMappings)
        //{
        //    int? index = null;

        //    for (int i = 0; i < sourceColumnHeaders.Length; i++)
        //    {
        //        string destinationColumnName = GetDestinationColumnName(sourceColumnHeaders[i], columnMappings);

        //        if (!string.IsNullOrEmpty(destinationColumnName) && destinationColumnName.Equals(name, StringComparison.OrdinalIgnoreCase))
        //        {
        //            index = i;
        //            break;
        //        }
        //    }

        //    if (!index.HasValue)
        //    {
        //        throw new ApplicationException($"Column '{name}' can not be found.");
        //    }

        //    return index.Value;
        //}

        //private string GetDestinationColumnName(string sourceColumnName, IList<LiteralColumnMappingDto> columnMappings)
        //{
        //    string destinationColumnName = null;

        //    var columnMappingDto =
        //        columnMappings.SingleOrDefault(mapping => mapping.SourceColumn.Equals(sourceColumnName, StringComparison.OrdinalIgnoreCase));

        //    if (columnMappingDto != null)
        //    {
        //        destinationColumnName = columnMappingDto.Destination;
        //    }

        //    return destinationColumnName;
        //}

        private IList<string> ParseDataRowDto(BatchProcessRecordEntity uploadRowDto)
        {
            var parser = CsvHelperFactory.CreateParser(uploadRowDto.Row);
            return parser.GetTrimmedValues();
        }
    }
}