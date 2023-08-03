using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BillingEngine.Billing;
using BillingEngine.Models.Ec2;
using CsvHelper;

namespace BillingEngine.Models.Billing
{
    public class MonthlyBill
    {
        public string CustomerId { get; }
        public string CustomerName { get; }

        public MonthYear MonthYear { get; }
        
        public List<MonthlyEc2InstanceUsage> MonthlyEc2InstanceUsages { get; }

        public MonthlyBill(string customerId, string customerName, MonthYear monthYear)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            MonthYear = monthYear;
            MonthlyEc2InstanceUsages = new List<MonthlyEc2InstanceUsage>();
        }

        public void AddMonthlyEc2Usages(List<MonthlyEc2InstanceUsage> monthlyEc2InstanceUsages)
        {
            MonthlyEc2InstanceUsages.AddRange(monthlyEc2InstanceUsages);
        }

        public List<AggregatedMonthlyEc2Usage> GetAggregatedMonthlyEc2Usages() //done
        {
            //Using MonthlyEc2InstanceUsages, compute List<AggregatedMonthlyEc2Usage>

            List<AggregatedMonthlyEc2Usage> aggregatedMonthlyEc2Usages = new List<AggregatedMonthlyEc2Usage>();

            foreach(var items in  MonthlyEc2InstanceUsages) 
            {
                bool flag = false;
                foreach( var monthlyusage in aggregatedMonthlyEc2Usages)
                {
                    if(monthlyusage.ResourceType.Equals(items.Ec2InstanceType.InstanceType))
                    {
                        monthlyusage.ids.Add(items.Ec2InstanceId);
                        double rate = items.Ec2InstanceType.CostPerHour;

                        foreach( var us in  items.Usages)
                        {
                            monthlyusage.TotalUsedTime += us.getusedtime();
                            monthlyusage.TotalBilledTime+= TimeSpan.FromHours(us.GetBillableHours());
                            monthlyusage.TotalAmount += rate* ((double)(us.GetBillableHours()));
                        }
                        flag = true;
                        break;
                    }
                }

                if(!flag)
                {
                    AggregatedMonthlyEc2Usage monthlyusage = new AggregatedMonthlyEc2Usage();

                    monthlyusage.ResourceType = items.Ec2InstanceType.InstanceType;
                    monthlyusage.ids.Add(items.Ec2InstanceId);
                    double rate = items.Ec2InstanceType.CostPerHour;

                    foreach (var us in items.Usages)
                    {
                        monthlyusage.TotalUsedTime += us.getusedtime();
                        monthlyusage.TotalBilledTime += TimeSpan.FromHours(us.GetBillableHours());
                        monthlyusage.TotalAmount += rate * ((double)(us.GetBillableHours()));
                    }
                }
            }

            foreach(var item in aggregatedMonthlyEc2Usages)
            {
                item.TotalResources = item.countinstance();
            }

            return aggregatedMonthlyEc2Usages;
        }

        public void ApplyDiscount(string ec2InstanceId, int discountedHours)
        {
            //Find matching object of type MonthlyEc2InstanceUsage from MonthlyEc2InstanceUsages
            // and then call monthlyEc2InstanceUsage.ApplyDiscount(discountedHours)
        }



        
        public double GetTotalAmount()
        {
            double total = 0.0;
            foreach(var items in MonthlyEc2InstanceUsages)
            {
                double rate = items.Ec2InstanceType.CostPerHour;
                int time=0;

                foreach(var use in items.Usages)
                {
                    time+=use.GetBillableHours();
                }

                total += rate * ((double)(time));
            }

            return total; 
        }

        public double GetTotalDiscount()
        {
            return 0.0;
        }

        public double GetAmountToBePaid()
        {
            return GetTotalAmount() - GetTotalDiscount();
        }

        /*
        public List<MonthlyEc2InstanceUsage> GetFreeTierEligibleInstanceUsagesOfType(OperatingSystem operatingSystem)
        {
            return MonthlyEc2InstanceUsages
                .Where(instanceUsage => instanceUsage.Ec2InstanceType.IsFreeTierEligible)
                .Where(instanceUsage => instanceUsage.Ec2InstanceType.OperatingSystem == operatingSystem)
                .ToList();
        }

        */
    }
}