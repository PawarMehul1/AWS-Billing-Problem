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

            //ec2instance list
            List<Ec2Instance> ec2instaces = new List<Ec2Instance>();

            for(int i=0; i< ec2ResourceUsageEventsForCustomer.Count; i++)
            {
               
                    bool find = false;
                    for(int j=0; j< ec2instaces.Count; j++)
                    {
                        if (ec2instaces[j].InstanceId.Equals(ec2ResourceUsageEventsForCustomer[i].Ec2InstanceId))
                        {
                            ec2instaces[j].Usages.Add(new ResourceUsageEvent(ec2ResourceUsageEventsForCustomer[i].UsedFrom, ec2ResourceUsageEventsForCustomer[i].UsedUntil));
                            find = true;
                            break;
                        }
                    }
                    if(!find)
                    {
                        Ec2InstanceType ec2insttype = new Ec2InstanceType();

                        for(int j=0; j< ec2InstanceTypes.Count; j++)
                        {
                            if (ec2InstanceTypes[j].InstanceType.Equals(ec2ResourceUsageEventsForCustomer[i].Ec2InstanceType))
                            {
                                ec2insttype= ec2InstanceTypes[j];
                            }
                        }

                        List<ResourceUsageEvent> usage = new List<ResourceUsageEvent>();
                        usage.Add(new ResourceUsageEvent(ec2ResourceUsageEventsForCustomer[i].UsedFrom, ec2ResourceUsageEventsForCustomer[i].UsedUntil));

                        Ec2Instance ec2inst = new Ec2Instance(ec2ResourceUsageEventsForCustomer[i].Ec2InstanceId,ec2insttype,usage);

                        ec2instaces.Add(ec2inst);
                    }
                
            }
           


            Customer cs = new Customer(parsedCustomerRecord.CustomerId,parsedCustomerRecord.CustomerName, ec2instaces);

            // Build customer object as well as associated composite objects, e.g. Ec2Instance, 
            return cs;
        }
    }
}