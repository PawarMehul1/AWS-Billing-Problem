using BillingEngine.Billing;
using BillingEngine.Printers;
using System;

namespace BillingEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            BillingService billingService = new BillingService();
            BillPrinter billPrinter = new BillPrinter();

            var monthlyBills = billingService.GenerateMonthlyBills(
                "C:/Users/india/Desktop/Internship/TestCases/Case1/Input/Customer.csv",
                "C:/Users/india/Desktop/Internship/TestCases/Case1/Input/AWSResourceTypes.csv",
                "C:/Users/india/Desktop/Internship/TestCases/Case1/Input/AWSCustomerUsage.csv",
                "C:/Users/india/Desktop/Internship/input/Region.csv"
            );
            Console.WriteLine(monthlyBills.Count);
            //monthlyBills.ForEach(monthlyBill => billPrinter.PrintBill(monthlyBill, "path/to/output/dir"));
        }
    }
}