using System.Collections.Generic;
using BillingEngine.Models;
using BillingEngine.Models.Billing;
using BillingEngine.Models.Ec2;
using static BillingEngine.Models.Ec2.Operatingsystem;

namespace BillingEngine.Billing
{
    public class DiscountService
    {
        private const int MaxFreeTierEligibleHours = 750;

        public int windowsUsage { get; set; }
        public int linuxUsage { get; set; }


        public DiscountService()
        {
            windowsUsage = 0;
            linuxUsage = 0;
        }

        public void ApplyDiscounts(AggregatedMonthlyEc2Usage aggregatedMonthlyEc2Usage,
            ResourceUsageEvent use,
            Operatingsystem OS, double rate)
        {

            if (OS == Operatingsystem.Windows && windowsUsage < 750)
            {
                int hours = use.GetBillableHours();

                if ((hours + windowsUsage) > 750)
                {
                    hours = 750 - windowsUsage;
                }

                windowsUsage += hours;

                addbilledhoursandtime(aggregatedMonthlyEc2Usage, hours, rate);

            }
            if (OS == Operatingsystem.Linux && linuxUsage < 750)
            {
                int hours = use.GetBillableHours();

                if ((hours + linuxUsage) > 750)
                {
                    hours = 750 - linuxUsage;
                }
                linuxUsage += hours;
                
                addbilledhoursandtime(aggregatedMonthlyEc2Usage, hours, rate);
  
            }
        }

        public void addbilledhoursandtime(AggregatedMonthlyEc2Usage aggregatedMonthlyEc2Usage,int hours,double rate)
        {
            aggregatedMonthlyEc2Usage.TotalDiscount += (double)(hours) * rate;
            aggregatedMonthlyEc2Usage.TotalDiscountedTime = TimeSpan.FromHours(hours);
        }

        private void DistributeFreeTierEligibleHoursAcrossInstances(
            List<MonthlyEc2InstanceUsage> monthlyFreeTierEligibleInstanceUsages,
            int maxFreeTierEligibleHours)
        {
            int remainingFreeTierEligibleHours = maxFreeTierEligibleHours;

            for (int i = 0; i < monthlyFreeTierEligibleInstanceUsages.Count && remainingFreeTierEligibleHours > 0; ++i)
            {
                var freeTierEligibleInstance = monthlyFreeTierEligibleInstanceUsages[i];
                var discountedHours = CalculateDiscountedHoursFor(
                    freeTierEligibleInstance,
                    remainingFreeTierEligibleHours
                );

                freeTierEligibleInstance.ApplyDiscount(discountedHours);

                remainingFreeTierEligibleHours -= discountedHours;
            }
        }

        private int CalculateDiscountedHoursFor(
            MonthlyEc2InstanceUsage monthlyFreeTierEligibleInstanceUsage,
            int availableFreeTierEligibleHours)
        {
            var totalBillableHours = monthlyFreeTierEligibleInstanceUsage.GetTotalBillableHours();
            return totalBillableHours < availableFreeTierEligibleHours
                ? totalBillableHours
                : availableFreeTierEligibleHours;
        }
    }
}