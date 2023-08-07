namespace BillingEngine.Models.Ec2
{
    public class Ec2InstanceType
    {
        public string InstanceType { get; }
        public double CostPerHour { get; }
        public Ec2Region Region { get; }
        public Operatingsystem OperatingSystem { get; }
        public BillingType billingType { get; }
        public bool IsFreeTierEligible { get; set; } 

        public bool isfreemonth { get; }

        public Ec2InstanceType() { }

        public Ec2InstanceType(Ec2InstanceType instance,bool isfree)
        {
            this.InstanceType = instance.InstanceType;
            this.CostPerHour = instance.CostPerHour;
            this.Region = instance.Region;
            this.OperatingSystem = instance.OperatingSystem;
            this.billingType = instance.billingType;
            this.IsFreeTierEligible = instance.IsFreeTierEligible;
            this.isfreemonth = isfree;
        }
        public Ec2InstanceType(string instancetype,double costperhour,Ec2Region region,Operatingsystem Os)  
        {
            this.InstanceType=instancetype;
            this.CostPerHour=costperhour;
            this.Region = region;
            this.OperatingSystem = Os;
            this.billingType = BillingType.OnDemand;
        }

        public Ec2InstanceType(string instancetype, double costperhour, Ec2Region region, Operatingsystem Os, BillingType bt)
        {
            this.InstanceType = instancetype;
            this.CostPerHour = costperhour;
            this.Region = region;
            this.OperatingSystem = Os;
            this.billingType = bt;
        }
    }
}