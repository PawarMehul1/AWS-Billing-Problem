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

            for (int i = 0; i < parsedEc2ResourceUsageTypeEventRecords.Count; i++)
            {

                bool find = false;
                for (int j = 0; j < ec2instaces.Count; j++)
                {
                    bool condition1 = ec2instaces[j].InstanceId.Equals(parsedEc2ResourceUsageTypeEventRecords[i].Ec2InstanceId);
                        bool condition2 = ec2instaces[j].InstanceType.OperatingSystem.Equals(parsedEc2ResourceUsageTypeEventRecords[i].OS);

                    if (condition1 && condition2)
                    {
                        DateTime starting = parsedEc2ResourceUsageTypeEventRecords[i].UsedFrom;
                        DateTime ending = parsedEc2ResourceUsageTypeEventRecords[i].UsedUntil;
                        ec2instaces[j].Usages.Add(new ResourceUsageEvent(starting,ending));
                        find = true;
                        break;
                    }
                }
                if (!find)
                {
                    Ec2InstanceType ec2insttype = new Ec2InstanceType();


                    for (int j = 0; j < ec2InstanceTypes.Count; j++)
                    {
                        bool condition1 = ec2InstanceTypes[j].InstanceType.Equals(parsedEc2ResourceUsageTypeEventRecords[i].Ec2InstanceType);
                        bool condition2 = ec2InstanceTypes[j].Region.Name.Equals(parsedEc2ResourceUsageTypeEventRecords[i].region);
                        bool condition3 = ec2InstanceTypes[j].OperatingSystem.Equals(parsedEc2ResourceUsageTypeEventRecords[i].OS);


                        if (condition1 && condition2 && condition3)
                        {
                            ec2insttype = ec2InstanceTypes[j];
                            break;
                        }
                    }


                    List<ResourceUsageEvent> usage = new List<ResourceUsageEvent>();

                    DateTime starting = parsedEc2ResourceUsageTypeEventRecords[i].UsedFrom;
                    DateTime ending = parsedEc2ResourceUsageTypeEventRecords[i].UsedUntil;

                    usage.Add(new ResourceUsageEvent(starting, ending));

                    string ec2instanceid = parsedEc2ResourceUsageTypeEventRecords[i].Ec2InstanceId;
                    Ec2Instance ec2inst = new Ec2Instance(ec2instanceid, ec2insttype, usage);

                    ec2instaces.Add(ec2inst);
                }

            }

            return ec2instaces;
        }
    }
}