using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

namespace Patient.Demographics.Common.Csv
{
    public static class CsvHelperFactory
    {
        private static readonly CsvConfiguration Configuration = new CsvConfiguration
        {
            Delimiter = ","
        };

        public static ICsvParser CreateParser(string text)
        {
            var stringReader = new StringReader(text);

            return new CsvParser(stringReader, Configuration);
        }

        public static ICsvWriter CreateWriter(TextWriter writer)
        {
            return new CsvWriter(writer, Configuration);
        }
    }
}
