namespace BillingEngine.Models.Ec2
{
    public class Ec2Region
    {
        public string Name { get; }

        public Ec2Region() { }
        public Ec2Region(string name) {  Name = name; }

    }
}