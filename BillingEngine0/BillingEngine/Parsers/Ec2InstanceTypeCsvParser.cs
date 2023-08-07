using System.Collections.Generic;
using System.Globalization;
using CsvHelper;
using System.IO;
using BillingEngine.Parsers.Models;
using System.Linq;

namespace BillingEngine.Parsers
{
    public class Ec2InstanceTypeCsvParser
    {

        public List<ParsedEc2InstanceType> Parse(string filePath)
        {
            using StreamReader streamReader = File.OpenText(filePath);
            using var csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);

            return csvReader.GetRecords<ParsedEc2InstanceType>().ToList();

        }
    }
}