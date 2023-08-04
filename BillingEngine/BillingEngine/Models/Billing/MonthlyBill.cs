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
            

            foreach(var monthly in MonthlyEc2InstanceUsages)
            {
                bool find = false;

                foreach(var aggrigatedinstance in aggregatedMonthlyEc2Usages)
                {
                    
                    if (aggrigatedinstance.ResourceType.Equals(monthly.Ec2InstanceType.InstanceType))
                    {
                        aggrigatedinstance.addid(monthly.Ec2InstanceId);
                        double rate = monthly.Ec2InstanceType.CostPerHour;
                        foreach (var use in monthly.Usages)
                        {
                            if (use.UsedFrom.Month == this.MonthYear.Month && use.UsedFrom.Year==this.MonthYear.Year)
                            {
                                aggrigatedinstance.TotalUsedTime += use.getusedtime();
                                aggrigatedinstance.TotalBilledTime += use.GetBillableHours() * 60 * 60;
                                aggrigatedinstance.TotalAmount += rate * (double)(use.GetBillableHours());
                            }
                        }

                        find = true;
                        break;
                    }
                }
                if(!find)
                {
                    AggregatedMonthlyEc2Usage aggregatedusage = new AggregatedMonthlyEc2Usage();

                    double rate = monthly.Ec2InstanceType.CostPerHour;
                    aggregatedusage.ResourceType = monthly.Ec2InstanceType.InstanceType;
                    aggregatedusage.addid(monthly.Ec2InstanceId);

                    foreach (var use in monthly.Usages)
                    { 
                            aggregatedusage.TotalUsedTime += use.getusedtime();
                            aggregatedusage.TotalBilledTime += use.GetBillableHours() * 60 * 60;
                            aggregatedusage.TotalAmount += rate * (double)(use.GetBillableHours());
                        
                    }

                    aggregatedMonthlyEc2Usages.Add (aggregatedusage);

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