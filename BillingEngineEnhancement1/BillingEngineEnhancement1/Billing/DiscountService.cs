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

        public void ApplyDiscounts(AggregatedMonthlyEc2Usage aggregatedusage,
            ResourceUsageEvent use,
            Operatingsystem OS, double rate)
        {
            if (OS == Operatingsystem.Windows && aggregatedusage.windowsuse < 750)
            {
                int hours = use.GetBillableHours();

                if ((hours + aggregatedusage.windowsuse) > 750)
                {
                    hours = 750 - aggregatedusage.windowsuse;
                }                   

                aggregatedusage.windowsuse +=hours;
                aggregatedusage.TotalDiscount += (double)(hours) * rate;
                aggregatedusage.TotalDiscountedTime = TimeSpan.FromHours(hours);
   
            }
            if (OS == Operatingsystem.Linux && aggregatedusage.linuxuse < 750)
            {
                int hours = use.GetBillableHours();

                if ((hours + aggregatedusage.linuxuse) > 750)
                {
                    hours = 750 - aggregatedusage.linuxuse;
                }
        
                aggregatedusage.linuxuse +=hours;
                aggregatedusage.TotalDiscount += (double)(hours) * rate;
                aggregatedusage.TotalDiscountedTime = TimeSpan.FromHours(hours);
  
            }
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