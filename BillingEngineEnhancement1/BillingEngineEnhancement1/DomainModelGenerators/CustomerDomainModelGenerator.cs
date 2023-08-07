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
            List<ParsedEc2ResourceUsageReserved> parsedEc2ResourceUsageReserveds,
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

            var customers=parsedCustomerRecords.Select(parsedCustomerRecord =>
                    GenerateCustomerModel(
                        parsedCustomerRecord,
                        parsedEc2ResourceUsageEventRecords.FindRecordsForCustomer(parsedCustomerRecord.CustomerId),
                        ec2InstanceTypes,
                        ec2Regions)
                )
                .ToList();

            foreach (var record in parsedEc2ResourceUsageReserveds)
            {
                bool find = false;
                foreach (var cust in customers)
                {
                    if (cust.CustomerId.Equals(record.CustomerId))  //
                    {

                        List<ResourceUsageEvent> Usages = new List<ResourceUsageEvent>();
                        Usages.Add(new ResourceUsageEvent(record.UsedFrom, record.UsedUntil));

                        string instancetype = record.Ec2InstanceType;
                        double rate = 0.0;

                        foreach (var item in parsedEc2InstanceTypes)
                        {
                            if (item.Ec2InstanceType.Equals(instancetype) && item.region.Equals(record.region))
                            {
                                rate = double.Parse(item.CostPerHourReserved);
                            }
                        }

                        Ec2Region region = new Ec2Region(record.region);
                        Operatingsystem os = record.OS;
                        BillingType bt = BillingType.Reserved;

                        Ec2InstanceType ec2InstanceType = new Ec2InstanceType(instancetype, rate, region, os, bt);


                        Ec2Instance ec2Instance = new Ec2Instance(record.Ec2InstanceId, ec2InstanceType, Usages);

                        cust.Ec2Instances.Add(ec2Instance);

                        find = true;
                        break;

                    }
                }
                if (!find)
                {
                    List<ResourceUsageEvent> Usages = new List<ResourceUsageEvent>();
                    Usages.Add(new ResourceUsageEvent(record.UsedFrom, record.UsedUntil));

                    string instancetype = record.Ec2InstanceType;
                    double rate = 0.0;

                    foreach (var item in parsedEc2InstanceTypes)
                    {
                        if (item.Ec2InstanceType.Equals(instancetype) && item.region.Equals(record.region))
                        {
                            rate = double.Parse(item.CostPerHourReserved);
                        }
                    }

                    Ec2Region region = new Ec2Region(record.region);
                    Operatingsystem os = record.OS;
                    BillingType bt = BillingType.Reserved;

                    Ec2InstanceType ec2InstanceType = new Ec2InstanceType(instancetype, rate, region, os, bt);


                    Ec2Instance ec2Instance = new Ec2Instance(record.Ec2InstanceId, ec2InstanceType, Usages);

                    List<Ec2Instance> list = new List<Ec2Instance>();
                    list.Add(ec2Instance);

                    string name = "";

                    foreach (var item in parsedCustomerRecords)
                    {
                        if (item.CustomerId.Equals(record.CustomerId))
                        {
                            name = item.CustomerName;
                        }
                    }

                    Customer cs = new Customer(record.CustomerId, name, list);
                    customers.Add(cs);
                }
            }

            return customers;
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
                    bool condition1 = ec2instaces[j].InstanceId.Equals(ec2ResourceUsageEventsForCustomer[i].Ec2InstanceId);
                    bool condition2= ec2instaces[j].InstanceType.OperatingSystem.Equals(ec2ResourceUsageEventsForCustomer[i].OS);

                    if (condition1 && condition2)
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
                        bool condition1 = ec2InstanceTypes[j].InstanceType.Equals(ec2ResourceUsageEventsForCustomer[i].Ec2InstanceType);
                        bool condition2 = ec2InstanceTypes[j].Region.Name.Equals(ec2ResourceUsageEventsForCustomer[i].region);
                        bool condition3 = ec2InstanceTypes[j].OperatingSystem.Equals(ec2ResourceUsageEventsForCustomer[i].OS);


                        if (condition1 && condition2 && condition3)
                            {
                                ec2insttype= ec2InstanceTypes[j];
                                break;
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