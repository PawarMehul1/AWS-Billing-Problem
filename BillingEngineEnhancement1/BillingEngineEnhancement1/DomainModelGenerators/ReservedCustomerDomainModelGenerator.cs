using BillingEngine.Models;
using BillingEngine.Models.Ec2;
using BillingEngine.Parsers;
using BillingEngine.Parsers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BillingEngineEnhancement1.DomainModelGenerators
{
    internal class ReservedCustomerDomainModelGenerator
    {
        public void AddReservedCustomersinCustomers(List<Customer> customers,
            List<ParsedCustomerRecord> parsedCustomerRecords,
            List<ParsedEc2InstanceType> parsedEc2InstanceTypes,
            List<ParsedEc2ResourceUsageReserved> parsedEc2ResourceUsageReserveds)
        {
            foreach (var record in parsedEc2ResourceUsageReserveds)
            {
                bool find = false;
                foreach (var cust in customers)
                {
                    if (cust.CustomerId.Equals(record.CustomerId))  
                    {
                        AddReservedCustomerInformation(customers,cust,
                            parsedCustomerRecords,
                            parsedEc2InstanceTypes,
                            record,true);

                        find = true;
                        break;

                    }
                }
                if (!find)
                {
                    Customer customer = new Customer();

                    AddReservedCustomerInformation(customers, customer,
                            parsedCustomerRecords,
                            parsedEc2InstanceTypes,
                            record, false);
                }
            }
        }

        public void AddReservedCustomerInformation(List<Customer> customers,Customer customer,
            List<ParsedCustomerRecord> parsedCustomerRecords,
            List<ParsedEc2InstanceType> parsedEc2InstanceTypes,
            ParsedEc2ResourceUsageReserved record,
            bool present)
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

                if (present)
                {
                    customer.Ec2Instances.Add(ec2Instance);
                }
                else 
                {
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
    }
}
