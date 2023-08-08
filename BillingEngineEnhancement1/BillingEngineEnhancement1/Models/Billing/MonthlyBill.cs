using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using BillingEngine.Billing;
using BillingEngine.Models.Billing;
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

        public List<AggregatedMonthlyEc2Usage> GetAggregatedMonthlyEc2Usages() 
        {
           
            List<AggregatedMonthlyEc2Usage> aggregatedMonthlyEc2Usages = new List<AggregatedMonthlyEc2Usage>();

            DiscountService discountService = new DiscountService();
            foreach(var MonthlyEc2InstanceUsage in MonthlyEc2InstanceUsages)
            {  
             
                    bool find = false;
                    bool isdiscountable = MonthlyEc2InstanceUsage.Ec2InstanceType.IsFreeTierEligible && MonthlyEc2InstanceUsage.Ec2InstanceType.isfreemonth;

                    foreach (var aggregatedMonthlyEc2Usage in aggregatedMonthlyEc2Usages)
                    {
                        bool issameResourcetype = aggregatedMonthlyEc2Usage.ResourceType.Equals(MonthlyEc2InstanceUsage.Ec2InstanceType.InstanceType);
                        bool issameRegion = aggregatedMonthlyEc2Usage.region.Name.Equals(MonthlyEc2InstanceUsage.Ec2InstanceType.Region.Name);
                        
                        if (issameResourcetype && issameRegion)
                        {

                        double rate = MonthlyEc2InstanceUsage.Ec2InstanceType.CostPerHour;
                        foreach (var Usage in MonthlyEc2InstanceUsage.Usages)
                        {
                            if (isdiscountable)
                            {
                                discountService.ApplyDiscounts(aggregatedMonthlyEc2Usage,Usage, MonthlyEc2InstanceUsage.Ec2InstanceType.OperatingSystem, rate);
                            }

                            aggregatedMonthlyEc2Usage.TotalUsedTime += Usage.getusedtime();
                            aggregatedMonthlyEc2Usage.TotalBilledTime += TimeSpan.FromHours(Usage.GetBillableHours());
                            aggregatedMonthlyEc2Usage.TotalAmount += rate * (double)(Usage.GetBillableHours());

                            if (Usage.GetBillableHours() > 0)
                            {
                                aggregatedMonthlyEc2Usage.addid(MonthlyEc2InstanceUsage.Ec2InstanceId);
                            }
                        }
                            find = true;
                            break;
                        }

                    }
                    if (!find)
                    {
                        AggregatedMonthlyEc2Usage aggregatedMonthlyEc2Usage = new AggregatedMonthlyEc2Usage();

                        double rate = MonthlyEc2InstanceUsage.Ec2InstanceType.CostPerHour;
                        aggregatedMonthlyEc2Usage.region = MonthlyEc2InstanceUsage.Ec2InstanceType.Region;
                        aggregatedMonthlyEc2Usage.ResourceType = MonthlyEc2InstanceUsage.Ec2InstanceType.InstanceType;

                        foreach (var Usage in MonthlyEc2InstanceUsage.Usages)
                        {
                            if (isdiscountable)
                            {
                                discountService.ApplyDiscounts(aggregatedMonthlyEc2Usage, Usage, MonthlyEc2InstanceUsage.Ec2InstanceType.OperatingSystem, rate);
                            }

                            aggregatedMonthlyEc2Usage.TotalUsedTime += Usage.getusedtime();
                            aggregatedMonthlyEc2Usage.TotalBilledTime += TimeSpan.FromHours(Usage.GetBillableHours());
                            aggregatedMonthlyEc2Usage.TotalAmount += rate * (double)(Usage.GetBillableHours());

                            if(Usage.GetBillableHours()>0)
                            {
                            aggregatedMonthlyEc2Usage.addid(MonthlyEc2InstanceUsage.Ec2InstanceId);
                        
                            }
                        }
                        aggregatedMonthlyEc2Usages.Add(aggregatedMonthlyEc2Usage);

                    } 
            }
            
            foreach(var aggregatedMonthlyEc2Usage in aggregatedMonthlyEc2Usages)
            {
                aggregatedMonthlyEc2Usage.TotalResources = aggregatedMonthlyEc2Usage.countinstance();
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
            double total = 0.0;

            foreach (var item in GetAggregatedMonthlyEc2Usages())
            {
                total += item.TotalDiscount;
            }

            return total;
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