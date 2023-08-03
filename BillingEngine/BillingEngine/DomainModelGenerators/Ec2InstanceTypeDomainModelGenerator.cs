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
            // Convert each object of type ParsedEc2InstanceType to Ec2InstanceType
            List<Ec2InstanceType> ec2InstanceTypes= new List<Ec2InstanceType>();

            for (int i = 0; i < parsedEc2InstanceTypes.Count; i++)
            {
                double cost = double.Parse(parsedEc2InstanceTypes[i].CostPerHour[1..]);

                Ec2InstanceType ec2InstanceType = new Ec2InstanceType(parsedEc2InstanceTypes[i].Ec2InstanceType, cost);

                ec2InstanceTypes.Add(ec2InstanceType);
                
            }
            return ec2InstanceTypes;
        }
    }
}