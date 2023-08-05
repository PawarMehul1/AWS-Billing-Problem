using BillingEngine.Models.Billing;
using System;
using System.Text;


namespace BillingEngine.Printers
{
    public class BillPrinter
    {
        public string PrintBill(MonthlyBill monthlyBill, string pathToOutputDir)
        {
            
            if (monthlyBill.GetTotalAmount() != 0)
            {
                string name = monthlyBill.CustomerId.Substring(0,4)+"-"+ monthlyBill.CustomerId.Substring(4) + "_";
                DateTime date = new DateTime(monthlyBill.MonthYear.Year, monthlyBill.MonthYear.Month, 1);

                name += (date.ToString("MMM")).ToUpper();
                name += "-" + monthlyBill.MonthYear.Year.ToString();

                String file = @"" + pathToOutputDir + name + ".csv";

                String separator = ",";
                StringBuilder output = new StringBuilder();

                string newLine="";

                newLine = string.Format("{0}", monthlyBill.CustomerName);
                output.AppendLine(string.Join(separator, newLine));

                string monthname="Bill for Month of "+date.ToString("MMMM")+" "+monthlyBill.MonthYear.Year.ToString();

                newLine = string.Format("{0}", monthname);
                output.AppendLine(string.Join(separator, newLine));

                string amount = Math.Round(monthlyBill.GetTotalAmount(), 4).ToString();
                newLine = string.Format("{0}", "Total amount : $"+amount);
                output.AppendLine(string.Join(separator, newLine));


                string heading = "Resource Type,Total Resources,Total Used Time (HH:mm:ss),Total Billed Time (HH:mm:ss),Rate (per hour),Total Amount";

                newLine = string.Format("{0}", heading);
                output.AppendLine(string.Join(separator, newLine));

                List<AggregatedMonthlyEc2Usage> lists = monthlyBill.GetAggregatedMonthlyEc2Usages();

                foreach(var aggrigatemonthbill in lists)
                {
                   if( aggrigatemonthbill.TotalAmount!=0)
                    {
                        newLine = string.Format("{0}", PrintBillItem(aggrigatemonthbill));
                        output.AppendLine(string.Join(separator, newLine));

                    }
                }

                try
                {
                    File.AppendAllText(file, output.ToString());

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);


                }

                return pathToOutputDir + name + ".csv";


            }

            return "";
        }

        public string timespan(TimeSpan ts)
        {

            int sec = ts.Seconds;
            int min = ts.Minutes;
            int hr = ts.Days*24+ts.Hours;


            return hr.ToString()+":"+min.ToString()+":"+sec.ToString();
        }

        private string PrintBillItem(AggregatedMonthlyEc2Usage aggregatedMonthlyEc2Usage)
        {

            string data = "";
            data += aggregatedMonthlyEc2Usage.ResourceType + ",";

            data += aggregatedMonthlyEc2Usage.TotalResources.ToString()+",";

            data += timespan(aggregatedMonthlyEc2Usage.TotalUsedTime) + ",";

            data += timespan(aggregatedMonthlyEc2Usage.TotalBilledTime) + ",$";
            
            double amount=Math.Round(aggregatedMonthlyEc2Usage.TotalAmount,4);
            TimeSpan ts = aggregatedMonthlyEc2Usage.TotalBilledTime;
            int hr =ts.Hours+ts.Days*24;

            data += (Math.Round(aggregatedMonthlyEc2Usage.TotalAmount / ((double)(hr)), 4)).ToString()+",$";
            data += amount.ToString();
            
            return data;

        }
    }
}