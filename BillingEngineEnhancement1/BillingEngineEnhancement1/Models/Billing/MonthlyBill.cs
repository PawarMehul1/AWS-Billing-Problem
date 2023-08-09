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
            foreach(var monthlyEc2InstanceUsage in MonthlyEc2InstanceUsages)
            {  
             
                    bool find = false;
                    bool isdiscountable = monthlyEc2InstanceUsage.Ec2InstanceType.IsFreeTierEligible && monthlyEc2InstanceUsage.Ec2InstanceType.isfreemonth;

                    foreach (var aggregatedMonthlyEc2Usage in aggregatedMonthlyEc2Usages)
                    {
                        bool issameResourcetype = aggregatedMonthlyEc2Usage.ResourceType.Equals(monthlyEc2InstanceUsage.Ec2InstanceType.InstanceType);
                        bool issameRegion = aggregatedMonthlyEc2Usage.region.Name.Equals(monthlyEc2InstanceUsage.Ec2InstanceType.Region.Name);
                        
                        if (issameResourcetype && issameRegion)
                        {
                            AddUsageandDiscount(monthlyEc2InstanceUsage,aggregatedMonthlyEc2Usage, 
                                discountService, isdiscountable);

                            find = true;
                            break;
                        }

                    }
                    if (!find)
                    {
                        AggregatedMonthlyEc2Usage aggregatedMonthlyEc2Usage = new AggregatedMonthlyEc2Usage();
                        
                        aggregatedMonthlyEc2Usage.region = monthlyEc2InstanceUsage.Ec2InstanceType.Region;
                        aggregatedMonthlyEc2Usage.ResourceType = monthlyEc2InstanceUsage.Ec2InstanceType.InstanceType;


                        AddUsageandDiscount(monthlyEc2InstanceUsage, aggregatedMonthlyEc2Usage,
                            discountService, isdiscountable);

                        aggregatedMonthlyEc2Usages.Add(aggregatedMonthlyEc2Usage);

                    } 
            }
            
            foreach(var aggregatedMonthlyEc2Usage in aggregatedMonthlyEc2Usages)
            {
                aggregatedMonthlyEc2Usage.TotalResources = aggregatedMonthlyEc2Usage.countinstance();
            }
            return aggregatedMonthlyEc2Usages;
        }

        public void AddUsageandDiscount(MonthlyEc2InstanceUsage monthlyEc2InstanceUsage,
                                        AggregatedMonthlyEc2Usage aggregatedMonthlyEc2Usage,
                                        DiscountService discountService,
                                        bool isdiscountable)
        {
            double rate = monthlyEc2InstanceUsage.Ec2InstanceType.CostPerHour;
            foreach (var Usage in monthlyEc2InstanceUsage.Usages)
            {
                if (isdiscountable)
                {
                    discountService.ApplyDiscounts(aggregatedMonthlyEc2Usage, Usage, monthlyEc2InstanceUsage.Ec2InstanceType.OperatingSystem, rate);
                }

                aggregatedMonthlyEc2Usage.TotalUsedTime += Usage.getusedtime();
                aggregatedMonthlyEc2Usage.TotalBilledTime += TimeSpan.FromHours(Usage.GetBillableHours());
                aggregatedMonthlyEc2Usage.TotalAmount += rate * (double)(Usage.GetBillableHours());

                if (Usage.GetBillableHours() > 0)
                {
                    aggregatedMonthlyEc2Usage.addid(monthlyEc2InstanceUsage.Ec2InstanceId);
                }
            }
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

    }
}