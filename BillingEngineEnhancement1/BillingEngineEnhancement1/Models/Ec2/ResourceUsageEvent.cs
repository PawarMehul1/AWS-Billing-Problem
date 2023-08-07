using System;

namespace BillingEngine.Models.Ec2
{
    public class ResourceUsageEvent
    {
        public DateTime UsedFrom { get; }
        
        public DateTime UsedUntil { get; }

        public ResourceUsageEvent() { }

        public ResourceUsageEvent(DateTime UsedFrom, DateTime UsedUntil)
        {
            this.UsedFrom = UsedFrom;
            this.UsedUntil = UsedUntil;
        }
        public TimeSpan getusedtime()
        {   
            TimeSpan ts = UsedUntil.Subtract(UsedFrom);
            return ts;
        }
        public int GetBillableHours()
        {
            var usedHours = UsedUntil.Subtract(UsedFrom).TotalHours;
            return Convert.ToInt32(Math.Ceiling(usedHours));
        }
    }
}