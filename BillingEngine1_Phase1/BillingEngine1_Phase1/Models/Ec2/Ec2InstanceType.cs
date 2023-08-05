namespace BillingEngine.Models.Ec2
{
    public class Ec2InstanceType
    {
        public string InstanceType { get; }
        public double CostPerHour { get; }
        public Ec2Region Region { get; }
        public Operatingsystem OperatingSystem { get; }
        public BillingType BillingType { get; }
        public bool IsFreeTierEligible { get; }

        public Ec2InstanceType() { }
        public Ec2InstanceType(string instancetype,double costperhour,Ec2Region region)  
        {
            this.InstanceType=instancetype;
            this.CostPerHour=costperhour;
            this.Region = region;
            this.IsFreeTierEligible = false;
        }
    }
}