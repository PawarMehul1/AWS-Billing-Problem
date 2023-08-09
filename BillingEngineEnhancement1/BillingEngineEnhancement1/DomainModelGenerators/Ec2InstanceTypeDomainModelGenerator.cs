using System;
using System.Collections.Generic;
using BillingEngine.Models.Ec2;
using BillingEngine.Parsers.Models;

namespace BillingEngine.DomainModelGenerators
{
    public class Ec2InstanceTypeDomainModelGenerator
    {

        public List<Ec2InstanceType> GenerateEc2InstanceTypes(List<ParsedEc2InstanceType> parsedEc2InstanceTypes) //done
        {
            List<Ec2InstanceType> ec2InstanceTypes = new List<Ec2InstanceType>();

            foreach(var parsedEc2InstanceType in parsedEc2InstanceTypes)
            {
                double CostPerHourOndemand = double.Parse(parsedEc2InstanceType.CostPerHourOndemand[1..]);

                Ec2Region region = new Ec2Region(parsedEc2InstanceType.region);
                var instancetype = parsedEc2InstanceType.Ec2InstanceType;


                Operatingsystem os = Operatingsystem.Windows;
                BillingType billingType = BillingType.OnDemand;

                Ec2InstanceType ec2InstanceType = new Ec2InstanceType(instancetype, CostPerHourOndemand, region, os, billingType);
                ec2InstanceTypes.Add(ec2InstanceType);

                os = Operatingsystem.Linux;
                ec2InstanceType = new Ec2InstanceType(instancetype, CostPerHourOndemand, region, os, billingType);
                ec2InstanceTypes.Add(ec2InstanceType);

            }
            return ec2InstanceTypes;
        }
    }
}