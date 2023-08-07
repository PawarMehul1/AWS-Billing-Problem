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
                "C:/Users/india/Desktop/AWS-Biling-Project/BillingEngine1_Phase1/input/Customer.csv",
                "C:/Users/india/Desktop/AWS-Biling-Project/BillingEngine1_Phase1/input/AWSResourceTypes.csv",
                "C:/Users/india/Desktop/AWS-Biling-Project/BillingEngine1_Phase1/input/AWSOnDemandResourceUsage.csv",
                "C:/Users/india/Desktop/AWS-Biling-Project/BillingEngine1_Phase1/input/Region.csv"
            );
            
            monthlyBills.ForEach(monthlyBill => billPrinter.PrintBill(monthlyBill, "C:/Users/india/Desktop/AWS-Biling-Project/BillingEngine1_Phase1/Result/"));
            
        }
    }
}