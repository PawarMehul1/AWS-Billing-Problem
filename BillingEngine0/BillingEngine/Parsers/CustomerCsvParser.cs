using System.Collections.Generic;
using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace BillingEngine.Parsers
{
    public class CustomerCsvParser
    {
        public List<ParsedCustomerRecord> Parse(string filePath)
        {
            using StreamReader streamReader = File.OpenText(filePath);
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            return csvReader.GetRecords<ParsedCustomerRecord>().ToList();
        }
    }
}