using System.Collections.Generic;
using System.Globalization;
using CsvHelper;
using System.IO;
using BillingEngine.Parsers.Models;
using System.Linq;

namespace BillingEngine.Parsers
{
    public class Ec2RegionCsvParser
    {
        public List<ParsedEc2Region> Parse(string regionPath)
        {
            using StreamReader streamReader = File.OpenText(regionPath);
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            return csvReader.GetRecords<ParsedEc2Region>().ToList();
        }
    }
}