using System;
using System.Collections.Generic;

namespace BillingEngine.Models.Billing
{
    public class AggregatedMonthlyEc2Usage
    {
        public string ResourceType { get; set; }
        public int TotalResources { get; set; }
        
        public HashSet<string> ids { get; set; }
        public int TotalBilledTime { get; set; }
        public int TotalUsedTime { get; set; }
        public TimeSpan TotalDiscountedTime { get; set; }
        
        public double TotalAmount { get; set; }
        public double TotalDiscount { get; set; }

        public AggregatedMonthlyEc2Usage()
        {
            ids = new HashSet<string>();
        }

        public void addid(string id)
        {
            this.ids.Add(id);
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