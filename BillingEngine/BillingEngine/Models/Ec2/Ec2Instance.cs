using System;
using System.Collections.Generic;
using System.Linq;
using BillingEngine.Models.Billing;

namespace BillingEngine.Models.Ec2
{
    public class Ec2Instance
    {
        public string InstanceId { get; }
        
        public Ec2InstanceType InstanceType { get; }

        public List<ResourceUsageEvent> Usages { get; }

        public Ec2Instance(string instanceId, Ec2InstanceType instanceType, List<ResourceUsageEvent> usages)
        {
            InstanceId = instanceId;
            InstanceType = instanceType;
            Usages = usages;
        }

        public MonthlyEc2InstanceUsage GetMonthlyEc2InstanceUsageForMonth(MonthYear monthYear)  //done
        {

            // Creates an instance of MonthlyEc2InstanceUsage by capturing usage events, applicable for a given month and year
            // For example, if Usages contain
            // 2021-05-10 to 2021-05-12 and
            // 2021-05-15 to 2021-05-17 and
            // 2021-05-25 to 2021-06-04 and
            // 2021-06-15 to 2021-06-17

            // Then newly constructed MonthlyEc2InstanceUsage for month 05/2021 (passed as argument) would include
            // 2021-05-10 to 2021-05-12 and
            // 2021-05-15 to 2021-05-17 and
            // 2021-05-25 to 2021-05-31

            MonthlyEc2InstanceUsage monthlyec2instanceusage = new MonthlyEc2InstanceUsage(InstanceId, InstanceType);

            for(int i=0; i<Usages.Count; i++)
            {
                DateTime from = Usages[i].UsedFrom;
                DateTime until = Usages[i].UsedUntil;


                int startmonth=(from.Year*100)+from.Month;
                int endmonth= (until.Year * 100) + until.Month;
                int currentMonth=(monthYear.Year*100)+monthYear.Month;

                if(currentMonth==endmonth && currentMonth==startmonth)
                {
                    ResourceUsageEvent resousageevnt = new ResourceUsageEvent(from,until);
                    monthlyec2instanceusage.AddEc2UsageEvent(resousageevnt);
                }
                else if(currentMonth<endmonth && currentMonth>startmonth) 
                {
                    DateTime datefrom = new DateTime(monthYear.Year,monthYear.Month,1,0,0,0);

                    int newyear= monthYear.Year;
                    int newmonth = monthYear.Month+1;

                    if(newmonth==13)
                    {
                        newmonth = 1;
                        newyear++;
                    }

                    DateTime dateuntil = new DateTime(newyear, newmonth, 1, 0, 0, 0);

                    ResourceUsageEvent resousageevnt = new ResourceUsageEvent(datefrom, dateuntil);
                    monthlyec2instanceusage.AddEc2UsageEvent(resousageevnt);

                }
                else if(currentMonth==startmonth && endmonth>currentMonth) 
                {
                    int newyear = monthYear.Year;
                    int newmonth = monthYear.Month + 1;

                    if (newmonth == 13)
                    {
                        newmonth = 1;
                        newyear++;
                    }

                    DateTime dateuntil = new DateTime(newyear, newmonth, 1, 0, 0, 0);

                    ResourceUsageEvent resousageevnt = new ResourceUsageEvent(from, dateuntil);
                    monthlyec2instanceusage.AddEc2UsageEvent(resousageevnt);
                }
                else if(currentMonth>startmonth && currentMonth==endmonth) 
                {
                    
                    DateTime datefrom = new DateTime(monthYear.Year, monthYear.Month, 1, 0, 0, 0);

                    ResourceUsageEvent resousageevnt = new ResourceUsageEvent(datefrom, until);
                    monthlyec2instanceusage.AddEc2UsageEvent(resousageevnt);
                }


            }

            return monthlyec2instanceusage;
        }

        public DateTime GetMinimumValueOfUsedFrom()
        {
            return Usages.Select(usage => usage.UsedFrom).Min();
        }
    }
}