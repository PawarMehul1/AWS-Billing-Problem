using BillingEngine.Models.Ec2;
using CsvHelper.Configuration.Attributes;
using System;

namespace BillingEngine.Parsers.Models
{
    public class ParsedEc2ResourceUsageReserved
    {
        [Name("Customer ID")]
        public string CustomerId { get; set; }


        [Name("EC2 Instance ID")]
        public string Ec2InstanceId { get; set; }

        [Name("EC2 Instance Type")]
        public string Ec2InstanceType { get; set; }

        [Name("Start Date")]
        public DateTime UsedFrom { get; set; }

        [Name("End Date")]
        public DateTime UsedUntil { get; set; }

        [Name("Region")]
        public string region { get; set; }

        [Name("OS")]
        public Operatingsystem OS { get; set; }


    }
}