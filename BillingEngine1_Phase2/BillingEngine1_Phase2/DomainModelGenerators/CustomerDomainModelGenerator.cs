using System.Collections.Generic;
using System.Linq;
using BillingEngine.Models;
using BillingEngine.Models.Ec2;
using BillingEngine.Parsers;
using BillingEngine.Parsers.Models;

namespace BillingEngine.DomainModelGenerators
{
    public class CustomerDomainModelGenerator
    {
        private readonly Ec2InstanceTypeDomainModelGenerator _ec2InstanceTypeDomainModelGenerator;
        private readonly Ec2InstanceDomainModelGenerator _ec2InstanceDomainModelGenerator;

        public CustomerDomainModelGenerator()
        {
            _ec2InstanceDomainModelGenerator = new Ec2InstanceDomainModelGenerator();
            _ec2InstanceTypeDomainModelGenerator = new Ec2InstanceTypeDomainModelGenerator();
        }

        public List<Customer> GenerateCustomerModels(
            List<ParsedCustomerRecord> parsedCustomerRecords,
            List<ParsedEc2InstanceType> parsedEc2InstanceTypes,
            List<ParsedEc2Region> parsedEc2Regions,
            List<ParsedEc2ResourceUsageEventRecord> parsedEc2ResourceUsageEventRecords)
        {
            List<Ec2InstanceType> ec2InstanceTypes = _ec2InstanceTypeDomainModelGenerator
                .GenerateEc2InstanceTypes(parsedEc2InstanceTypes);


            foreach(var instancetype in ec2InstanceTypes)
            {
                foreach(var regions in parsedEc2Regions)
                {
                    if(regions.RegionName.Equals(instancetype.Region.Name) && regions.FreeTierEligibleInstanceType.Equals(instancetype.InstanceType))
                    {
                        instancetype.IsFreeTierEligible = true;
                        break;
                    }
                }
            }

            //Generate Ec2Region instances by defining Ec2RegionDomainModelGenerator
            List<Ec2Region> ec2Regions = new List<Ec2Region>();

            return parsedCustomerRecords.Select(parsedCustomerRecord =>
                    GenerateCustomerModel(
                        parsedCustomerRecord,
                        parsedEc2ResourceUsageEventRecords.FindRecordsForCustomer(parsedCustomerRecord.CustomerId),
                        ec2InstanceTypes,
                        ec2Regions)
                )
                .ToList();
        }

        private Customer GenerateCustomerModel(
            ParsedCustomerRecord parsedCustomerRecord,
            List<ParsedEc2ResourceUsageEventRecord> ec2ResourceUsageEventsForCustomer,
            List<Ec2InstanceType> ec2InstanceTypes,
            List<Ec2Region> ec2Regions) //done
        {


            List<Ec2Instance> ec2instaces = _ec2InstanceDomainModelGenerator.GenerateEc2InstanceModels(
             ec2ResourceUsageEventsForCustomer,
             ec2InstanceTypes);
            Customer cs = new Customer(parsedCustomerRecord.CustomerId,parsedCustomerRecord.CustomerName, ec2instaces);

            // Build customer object as well as associated composite objects, e.g. Ec2Instance, 
            return cs;
        }
    }
}