using System.Collections.Generic;
using System.Linq;
using BillingEngine.Models.Ec2;

namespace BillingEngine.Models.Billing
{
    public class MonthlyEc2InstanceUsage
    {
        public string Ec2InstanceId { get; }
        
        public Ec2InstanceType Ec2InstanceType { get; }
        
        public List<ResourceUsageEvent> Usages { get; }
        
        public int DiscountedHours { get; private set; }

        public MonthlyEc2InstanceUsage()
        {
            Usages = new List<ResourceUsageEvent>();
        }

        public MonthlyEc2InstanceUsage(string instanceId, Ec2InstanceType instanceType,List<ResourceUsageEvent> usages)
        {
            this.Ec2InstanceId= instanceId;
            this.Ec2InstanceType = instanceType;
            this.Usages = usages;
        }
        public MonthlyEc2InstanceUsage(string instanceId, Ec2InstanceType instanceType)
        {
            this.Ec2InstanceId = instanceId;
            this.Ec2InstanceType = instanceType;
            Usages = new List<ResourceUsageEvent>();
        }

        public void AddEc2UsageEvent(ResourceUsageEvent usageEvent)
        {
            Usages.Add(usageEvent);
        }

        public void ApplyDiscount(int discountedHours)
        {
            DiscountedHours = discountedHours;
        }

        public int GetTotalBillableHours()
        {
            return Usages.Select(usage => usage.GetBillableHours()).Sum();
        }
    }
}