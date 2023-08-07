using System.Collections.Generic;
using System.Linq;
using BillingEngine.Parsers.Models;

namespace BillingEngine.DomainModelGenerators
{
    public static class DomainModelGeneratorExtensions
    {
        // This is an extension method (used to extend the functionality of some existing class)
        public static List<ParsedEc2ResourceUsageEventRecord> FindRecordsForCustomer(
            this List<ParsedEc2ResourceUsageEventRecord> parsedEc2ResourceUsageEventRecords,
            string customerId)
        {
            return parsedEc2ResourceUsageEventRecords
                .Where(record => record.CustomerId == customerId)
                .ToList();
        }
    }
}