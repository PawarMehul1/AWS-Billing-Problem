using BillingEngine.Models.Billing;
using System;
using System.Text;


namespace BillingEngine.Printers
{
    public class BillPrinter
    {
        public string PrintBill(MonthlyBill monthlyBill, string pathToOutputDir)
        {
            //This method will just print the bill by generating CSV and returns file path of that csv.
            // Print header information like customer name, month, year and then print
            List<AggregatedMonthlyEc2Usage> lists = monthlyBill.GetAggregatedMonthlyEc2Usages();


            // monthlyBill.GetTotalAmount();
            Console.Write(monthlyBill.MonthYear.Month + " ");
            Console.Write(monthlyBill.MonthYear.Year+ " ");
            Console.WriteLine(monthlyBill.GetTotalAmount());
            // monthlyBill.GetTotalDiscount();
            // monthlyBill.GetAmountToBePaid();

            //Now print itemized bill


                monthlyBill.GetAggregatedMonthlyEc2Usages().ForEach(PrintBillItem);

            Console.WriteLine();
            

            //Return path of generated CSV
            return "";
        }

        public string timespan(int ts)
        {
            int sec = ts%60;
            ts /= 60;

            int min = ts % 60;
            ts/= 60;
            int hr = ts;


            return hr.ToString()+":"+min.ToString()+":"+sec.ToString();
        }

        private void PrintBillItem(AggregatedMonthlyEc2Usage aggregatedMonthlyEc2Usage)
        {
            
            Console.Write(aggregatedMonthlyEc2Usage.ResourceType+" ");

            Console.Write(aggregatedMonthlyEc2Usage.TotalResources + " ");

            Console.Write( timespan(aggregatedMonthlyEc2Usage.TotalUsedTime) + " ");

            Console.Write(timespan(aggregatedMonthlyEc2Usage.TotalBilledTime)+ " ");

            Console.WriteLine(aggregatedMonthlyEc2Usage.TotalAmount);

        }
    }
}