using System.Collections.Generic;
using System.Linq;
using CsvHelper;

namespace Patient.Demographics.Common.Csv
{
    public static class CsvParserExtensions
    {
        public static List<string> GetTrimmedValues(this ICsvParser csvParser)
        {
            var rawValues = csvParser.Read();

            if (rawValues == null)
            {
                return new List<string>();
            }

            return rawValues.Select(value => value.Trim()).ToList();
        }
    }
}