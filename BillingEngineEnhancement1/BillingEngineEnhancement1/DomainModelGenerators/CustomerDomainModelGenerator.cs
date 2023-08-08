using System.Collections.Generic;
using System.Linq;
using BillingEngine.Models;
using BillingEngine.Models.Ec2;
using BillingEngine.Parsers;
using BillingEngine.Parsers.Models;
using BillingEngineEnhancement1.DomainModelGenerators;

namespace BillingEngine.DomainModelGenerators
{
    public class CustomerDomainModelGenerator
    {
        private readonly Ec2InstanceTypeDomainModelGenerator _ec2InstanceTypeDomainModelGenerator;
        private readonly Ec2InstanceDomainModelGenerator _ec2InstanceDomainModelGenerator;
        private readonly ReservedCustomerDomainModelGenerator _reservedCustomerDomainModelGenerator;

        public CustomerDomainModelGenerator()
        {
            _ec2InstanceDomainModelGenerator = new Ec2InstanceDomainModelGenerator();
            _ec2InstanceTypeDomainModelGenerator = new Ec2InstanceTypeDomainModelGenerator();
            _reservedCustomerDomainModelGenerator=new ReservedCustomerDomainModelGenerator();
        }

        public List<Customer> GenerateCustomerModels(
            List<ParsedCustomerRecord> parsedCustomerRecords,
            List<ParsedEc2InstanceType> parsedEc2InstanceTypes,
            List<ParsedEc2Region> parsedEc2Regions,
            List<ParsedEc2ResourceUsageReserved> parsedEc2ResourceUsageReserveds,
            List<ParsedEc2ResourceUsageEventRecord> parsedEc2ResourceUsageEventRecords)
        {
            List<Ec2InstanceType> ec2InstanceTypes = _ec2InstanceTypeDomainModelGenerator
                .GenerateEc2InstanceTypes(parsedEc2InstanceTypes);


            foreach(var ec2instancetype in ec2InstanceTypes)
            {
                foreach(var ec2region in parsedEc2Regions)
                {
                    if(ec2region.RegionName.Equals(ec2instancetype.Region.Name) && ec2region.FreeTierEligibleInstanceType.Equals(ec2instancetype.InstanceType))
                    {
                        ec2instancetype.IsFreeTierEligible = true;
                        break;
                    }
                }
            }

            List<Ec2Region> ec2Regions = new List<Ec2Region>();

            var customers=parsedCustomerRecords.Select(parsedCustomerRecord =>
                    GenerateCustomerModel(
                        parsedCustomerRecord,
                        parsedEc2ResourceUsageEventRecords.FindRecordsForCustomer(parsedCustomerRecord.CustomerId),
                        ec2InstanceTypes,
                        ec2Regions)
                )
                .ToList();

          
            _reservedCustomerDomainModelGenerator.AddReservedCustomersinCustomers(customers,
                parsedCustomerRecords, 
                parsedEc2InstanceTypes, 
                parsedEc2ResourceUsageReserveds);



            return customers;
        }

        private Customer GenerateCustomerModel(
            ParsedCustomerRecord parsedCustomerRecord,
            List<ParsedEc2ResourceUsageEventRecord> ec2ResourceUsageEventsForCustomer,
            List<Ec2InstanceType> ec2InstanceTypes,
            List<Ec2Region> ec2Regions)
        {

            List<Ec2Instance> ec2instaces = _ec2InstanceDomainModelGenerator.GenerateEc2InstanceModels(ec2ResourceUsageEventsForCustomer,ec2InstanceTypes);

            Customer customer = new Customer(parsedCustomerRecord.CustomerId,parsedCustomerRecord.CustomerName, ec2instaces);

            return customer;
        }
    }
}