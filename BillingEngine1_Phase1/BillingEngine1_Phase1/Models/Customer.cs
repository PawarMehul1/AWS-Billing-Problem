using System;
using System.Collections.Generic;
using System.Linq;
using BillingEngine.Models.Billing;
using BillingEngine.Models.Ec2;

namespace BillingEngine.Models
{
    public class Customer
    {
        public string CustomerId { get; }
        public string CustomerName { get; }
        
        public List<Ec2Instance> Ec2Instances { get; }

        public Customer()
        {
            Ec2Instances = new List<Ec2Instance>();
        }

        public Customer(string customerId, string customerName, List<Ec2Instance> ec2Instances)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            Ec2Instances = ec2Instances;
        }

        public List<MonthYear> GetDistinctMonthYears()
        {
            HashSet<int> months = new HashSet<int>();

            foreach(var instance in Ec2Instances)
            {
                foreach(var use in instance.Usages)
                {
                    int starting = (use.UsedFrom.Year) * 100 + use.UsedFrom.Month;
                    int ending = (use.UsedUntil.Year) * 100 + use.UsedUntil.Month;

                    for(int i=starting; i<=ending; i++)
                    {
                        if((i%100)==13)
                        {
                            i -= 12;
                            i += 100;
                        }
                        months.Add(i);
                    }
                }
            }

            List<MonthYear> distintMonths = new List<MonthYear>();
            foreach(var mon in months)
            {
                MonthYear month = new MonthYear(mon%100,mon/100);
                distintMonths.Add(month);
            }
            return distintMonths;
        }

        public List<MonthlyEc2InstanceUsage> GetMonthlyEc2InstanceUsagesForMonth(MonthYear monthYear)  //done
        {
            // Using List<Ec2Instance> , construct  List<MonthlyEc2InstanceUsage>
            // by calling ec2Instance.GetUsageInMonth(monthYear)

            List<MonthlyEc2InstanceUsage> monthlyec2instnceusage = new List<MonthlyEc2InstanceUsage>();

            foreach(var ec2instance in Ec2Instances)
            {

                monthlyec2instnceusage.Add(ec2instance.GetMonthlyEc2InstanceUsageForMonth(monthYear));
            }

            return  monthlyec2instnceusage;
        }

        public DateTime GetJoiningDate()
        {
            return Ec2Instances
                .Select(instance => instance.GetMinimumValueOfUsedFrom())
                .Min();
        }
    }
}