using System;
using System.Collections.Generic;

namespace BillingEngine.Models.Billing
{
    public class AggregatedMonthlyEc2Usage
    {
        public string ResourceType { get; set; }
        public int TotalResources { get; set; }
        
        public HashSet<String> ids { get; set; }
        public TimeSpan TotalBilledTime { get; set; }
        public TimeSpan TotalUsedTime { get; set; }
        public TimeSpan TotalDiscountedTime { get; set; }
        
        public double TotalAmount { get; set; }
        public double TotalDiscount { get; set; }

        public AggregatedMonthlyEc2Usage()
        {
            
        }

        public int countinstance()
        {
            return ids.Count;
        }

        public double GetActualAmountToBePaid()
        {
            return TotalAmount - TotalDiscount;
        }
        
    }
}