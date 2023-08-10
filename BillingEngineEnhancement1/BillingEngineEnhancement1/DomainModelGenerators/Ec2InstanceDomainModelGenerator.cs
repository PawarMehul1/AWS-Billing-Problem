using System.Collections.Generic;
using BillingEngine.Models.Ec2;
using BillingEngine.Parsers.Models;

namespace BillingEngine.DomainModelGenerators
{
    public class Ec2InstanceDomainModelGenerator
    {
        public List<Ec2Instance> GenerateEc2InstanceModels(
            List<ParsedEc2ResourceUsageEventRecord> parsedEc2ResourceUsageTypeEventRecords,
            List<Ec2InstanceType> ec2InstanceTypes)
        {

            List<Ec2Instance> ec2instaces = new List<Ec2Instance>();

            foreach(var ResourceUsageTypeEventRecord in parsedEc2ResourceUsageTypeEventRecords)
            {
                bool ec2instacepresent = false;

                foreach(var ec2instance in ec2instaces)
                {
                    string userInstanceId = ResourceUsageTypeEventRecord.Ec2InstanceId;
                   
                    bool issameIntancetype = ec2instance.InstanceId.Equals(userInstanceId);
                    bool issameos = ec2instance.InstanceType.OperatingSystem.Equals(ResourceUsageTypeEventRecord.OS);
                    bool issametype = ec2instance.InstanceType.InstanceType.Equals(ResourceUsageTypeEventRecord.Ec2InstanceType);
                    bool issameregion = ec2instance.InstanceType.Region.Name.Equals(ResourceUsageTypeEventRecord.region);

                    if (issameIntancetype && issameregion && issametype && issameos)
                    {
                        DateTime starting = ResourceUsageTypeEventRecord.UsedFrom;
                        DateTime ending = ResourceUsageTypeEventRecord.UsedUntil;

                        ec2instance.Usages.Add(new ResourceUsageEvent(starting,ending));

                        ec2instacepresent = true;
                        break;
                    }
                }
                if (!ec2instacepresent)
                {
                    Ec2InstanceType ec2instncetype = new Ec2InstanceType();

                    foreach(var ec2InstanceType in ec2InstanceTypes)
                    {
                        bool issameIntancetype = ec2InstanceType.InstanceType.Equals(ResourceUsageTypeEventRecord.Ec2InstanceType);
                        bool issameregion = ec2InstanceType.Region.Name.Equals(ResourceUsageTypeEventRecord.region);
                        bool issameOS = ec2InstanceType.OperatingSystem.Equals(ResourceUsageTypeEventRecord.OS);

                        if (issameIntancetype && issameregion && issameOS)
                        {
                            ec2instncetype = ec2InstanceType;
                            break;
                        }
                    }

                    List<ResourceUsageEvent> usage = new List<ResourceUsageEvent>();

                    DateTime startDate = ResourceUsageTypeEventRecord.UsedFrom;
                    DateTime endDate = ResourceUsageTypeEventRecord.UsedUntil;

                    usage.Add(new ResourceUsageEvent(startDate, endDate));

                    string ec2instanceid = ResourceUsageTypeEventRecord.Ec2InstanceId;

                    Ec2Instance ec2instance = new Ec2Instance(ec2instanceid, ec2instncetype, usage);

                    ec2instaces.Add(ec2instance);
                }

            }

            return ec2instaces;
        }
    }
}