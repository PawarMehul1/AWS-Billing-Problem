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
        public int getusedtime()
        {
            
            TimeSpan ts = UsedUntil.Subtract(UsedFrom);
            int sec = ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Seconds + ts.Minutes * 60;
            return sec;
        }
        public int GetBillableHours()
        {
            var usedHours = UsedUntil.Subtract(UsedFrom).TotalHours;
            return Convert.ToInt32(Math.Ceiling(usedHours));
        }
    }
}