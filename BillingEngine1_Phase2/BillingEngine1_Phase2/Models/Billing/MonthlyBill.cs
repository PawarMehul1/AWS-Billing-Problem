using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
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

        public int windowsusage { get; set; }

        public int linuxusage { get; set; }

        public MonthYear MonthYear { get; }
        
        public List<MonthlyEc2InstanceUsage> MonthlyEc2InstanceUsages { get; }

        public MonthlyBill(string customerId, string customerName, MonthYear monthYear)
        {
            CustomerId = customerId;
            CustomerName = customerName;
            MonthYear = monthYear;
            MonthlyEc2InstanceUsages = new List<MonthlyEc2InstanceUsage>();
            windowsusage = 0;
            linuxusage = 0;
        }

        public void AddMonthlyEc2Usages(List<MonthlyEc2InstanceUsage> monthlyEc2InstanceUsages)
        {
            MonthlyEc2InstanceUsages.AddRange(monthlyEc2InstanceUsages);
        }

        public List<AggregatedMonthlyEc2Usage> GetAggregatedMonthlyEc2Usages() 
        {
            //Using MonthlyEc2InstanceUsages, compute List<AggregatedMonthlyEc2Usage>
            windowsusage = 0;
            linuxusage = 0;

            List<AggregatedMonthlyEc2Usage> aggregatedMonthlyEc2Usages = new List<AggregatedMonthlyEc2Usage>();
            
            
            foreach(var monthly in MonthlyEc2InstanceUsages)
            {

                    bool find = false;
                    bool isfree = monthly.Ec2InstanceType.IsFreeTierEligible && monthly.Ec2InstanceType.isfreemonth;

                    foreach (var aggrigatedinstance in aggregatedMonthlyEc2Usages)
                    {
                        bool condition1 = aggrigatedinstance.ResourceType.Equals(monthly.Ec2InstanceType.InstanceType);
                        bool condition2 = aggrigatedinstance.region.Name.Equals(monthly.Ec2InstanceType.Region.Name);
                        if (condition1 && condition2)
                        {
                            
                            double rate = monthly.Ec2InstanceType.CostPerHour;
                            foreach (var use in monthly.Usages)
                            {
                                    if (isfree)
                                    {
                                        adddiscount(aggrigatedinstance, use, monthly.Ec2InstanceType.OperatingSystem, rate);
                                    }
                                    aggrigatedinstance.TotalUsedTime += use.getusedtime();
                                    aggrigatedinstance.TotalBilledTime += TimeSpan.FromHours(use.GetBillableHours());
                                    aggrigatedinstance.TotalAmount += rate * (double)(use.GetBillableHours());

                            if (use.GetBillableHours() > 0)
                            {
                                aggrigatedinstance.addid(monthly.Ec2InstanceId);

                            }
                        }

                            find = true;
                            break;
                        }

                    }
                    if (!find)
                    {
                        AggregatedMonthlyEc2Usage aggregatedusage = new AggregatedMonthlyEc2Usage();

                        double rate = monthly.Ec2InstanceType.CostPerHour;
                        aggregatedusage.region = monthly.Ec2InstanceType.Region;
                        aggregatedusage.ResourceType = monthly.Ec2InstanceType.InstanceType;
                        

                        foreach (var use in monthly.Usages)
                        {
                            if (isfree)
                            {
                               adddiscount(aggregatedusage, use, monthly.Ec2InstanceType.OperatingSystem, rate);
                            }

                            aggregatedusage.TotalUsedTime += use.getusedtime();
                            aggregatedusage.TotalBilledTime += TimeSpan.FromHours(use.GetBillableHours());
                            aggregatedusage.TotalAmount += rate * (double)(use.GetBillableHours());

                        if (use.GetBillableHours() > 0)
                        {
                            aggregatedusage.addid(monthly.Ec2InstanceId);

                        }
                    }
                        aggregatedMonthlyEc2Usages.Add(aggregatedusage);

                    }
             
            }
            
            foreach(var item in aggregatedMonthlyEc2Usages)
            {
                item.TotalResources = item.countinstance();
            }

            return aggregatedMonthlyEc2Usages;
        }

        public void adddiscount(AggregatedMonthlyEc2Usage aggregatedusage, ResourceUsageEvent use,Operatingsystem OS,double rate)
        {
            if(OS==Operatingsystem.Windows && windowsusage < 750)
            {
                int hours = use.GetBillableHours();

                if((hours+ windowsusage) <=750)
                {
                    windowsusage += hours;
                    aggregatedusage.TotalDiscount += (double)(hours) * rate;
                    aggregatedusage.TotalDiscountedTime= TimeSpan.FromHours(hours);
                }
                else
                {
                    hours = 750-windowsusage;

                    windowsusage = 750;
                    aggregatedusage.TotalDiscount += (double)(hours) * rate;
                    aggregatedusage.TotalDiscountedTime = TimeSpan.FromHours(hours);

                }
            }
            if (OS == Operatingsystem.Linux && linuxusage < 750)
            {
                int hours = use.GetBillableHours();

                if ((hours + linuxusage) <= 750)
                {
                    linuxusage += hours;
                    aggregatedusage.TotalDiscount += (double)(hours) * rate;
                    aggregatedusage.TotalDiscountedTime = TimeSpan.FromHours(hours);
                }
                else
                {
                    hours = 750- linuxusage;

                    linuxusage = 750;
                    aggregatedusage.TotalDiscount += (double)(hours) * rate;
                    aggregatedusage.TotalDiscountedTime = TimeSpan.FromHours(hours);

                }
            }
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