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
            foreach (var Ec2ResourceUsageReserved in parsedEc2ResourceUsageReserveds)
            {
                bool find = false;
                foreach (var customer in customers)
                {
                    if (customer.CustomerId.Equals(Ec2ResourceUsageReserved.CustomerId))  
                    {
                        AddReservedCustomerInformation(customers, customer,
                            parsedCustomerRecords,
                            parsedEc2InstanceTypes,
                            Ec2ResourceUsageReserved, true);

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
                            Ec2ResourceUsageReserved, false);
                }
            }
        }

        public void AddReservedCustomerInformation(List<Customer> customers,Customer customer,
            List<ParsedCustomerRecord> parsedCustomerRecords,
            List<ParsedEc2InstanceType> parsedEc2InstanceTypes,
            ParsedEc2ResourceUsageReserved Ec2ResourceUsageReserved,
            bool customerispresent)
        {
            
                List<ResourceUsageEvent> Usages = new List<ResourceUsageEvent>();
                Usages.Add(new ResourceUsageEvent(Ec2ResourceUsageReserved.UsedFrom,
                    Ec2ResourceUsageReserved.UsedUntil));

                string instancetype = Ec2ResourceUsageReserved.Ec2InstanceType;
                double rate = 0.0;

                foreach (var Ec2InstanceTypes in parsedEc2InstanceTypes)
                {
                    if (Ec2InstanceTypes.Ec2InstanceType.Equals(instancetype) && Ec2InstanceTypes.region.Equals(Ec2ResourceUsageReserved.region))
                    {
                        rate = double.Parse(Ec2InstanceTypes.CostPerHourReserved);
                    }
                }

                Ec2Region region = new Ec2Region(Ec2ResourceUsageReserved.region);
                Operatingsystem os = Ec2ResourceUsageReserved.OS;
                BillingType billingtype = BillingType.Reserved;

                Ec2InstanceType ec2InstanceType = new Ec2InstanceType(instancetype, rate, region, os, billingtype);

                Ec2Instance ec2Instance = new Ec2Instance(Ec2ResourceUsageReserved.Ec2InstanceId, ec2InstanceType, Usages);

                if (customerispresent)
                {
                    customer.Ec2Instances.Add(ec2Instance);
                }
                else 
                {
                    List<Ec2Instance> list = new List<Ec2Instance>();
                    list.Add(ec2Instance);

                    string customername = "";
                    foreach (var CustomerRecords in parsedCustomerRecords)
                    {
                        if (CustomerRecords.CustomerId.Equals(Ec2ResourceUsageReserved.CustomerId))
                        {
                            customername = CustomerRecords.CustomerName;
                        }
                    }

                    Customer cs = new Customer(Ec2ResourceUsageReserved.CustomerId, customername, list);
                    customers.Add(cs);
                }   
        }
    }
}
