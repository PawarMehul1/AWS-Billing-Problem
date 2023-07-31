using Enhancement0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enhancement0
{
    internal class OutputCSV
    {
        string CustId;
        string username;
        string monthname;
        double total;
        List<TypeCharge> typecharge = new List<TypeCharge>();

        public OutputCSV(string CustId, string month, double total, List<TypeCharge> typcrg)
        {
            this.total = total;
            this.typecharge = typcrg;
            this.CustId = CustId;
            this.username = getusername(CustId);
            this.monthname = month;

            generateCSV();
        }

        public string getusername(string CustId)
        {
            CustId = CustId.Substring(0, 4) + "-" + CustId.Substring(4);

            string Customer_path = "C:/Users/india/Desktop/Internship/TestCases/Case1/Input/Customer.csv";
            string[] Customers = File.ReadAllLines(Customer_path);

            String ans = "";
            for (int i = 1; i < Customers.Length; i++)
            {
                string record = Customers[i];

                string[] delimetrs = { "\"", "," };
                String[] values = record.Split(delimetrs, StringSplitOptions.None);


                if (values[4].Equals(CustId))
                {
                    ans = values[7];
                }
            }

            return ans;
        }
        public string getmonthname(string month)
        {
            string[] strs = month.Split('-');
            int Month = int.Parse(strs[1]);

            DateTime date = new DateTime(2023, Month, 1);

            return (date.ToString("MMMM")) + " " + strs[0];
        }

        public string gethalfname(string month)
        {
            string[] strs = month.Split('-');

            int Month = int.Parse(strs[1]);
            DateTime date = new DateTime(2023, Month, 1);

            return (date.ToString("MMM")).ToUpper() + "-" + strs[0];
        }

        public void generateCSV()
        {
            string OutputName = this.CustId.Substring(0, 4) + "-" + this.CustId.Substring(4) + "_";
            OutputName += gethalfname(this.monthname);

            String file = @"C:\Users\india\Desktop\Internship\TestCases\Case1\Result\" + OutputName + ".csv";

            String separator = ",";
            StringBuilder output = new StringBuilder();


            string newLine = string.Format("{0}", this.username);
            output.AppendLine(string.Join(separator, newLine));

            newLine = string.Format("{0}", "Bill for month of " + getmonthname(this.monthname));
            output.AppendLine(string.Join(separator, newLine));

            newLine = string.Format("{0}", "Total Amount : $" + Math.Round(this.total, 4));
            output.AppendLine(string.Join(separator, newLine));


            string heading = "Resource Type,Total Resources,Total Used Time (HH:mm:ss),Total Billed Time (HH:mm:ss),Rate (per hour),Total Amount";

            newLine = string.Format("{0}", heading);
            output.AppendLine(string.Join(separator, newLine));

            string[] str;

            for (int i = 0; i < this.typecharge.Count; i++)
            {
                str = this.typecharge[i].deatils();
                string values = str[0];

                for (int j = 1; j < str.Length; j++)
                {
                    values += "," + str[j];
                }

                newLine = string.Format("{0}", values);
                output.AppendLine(string.Join(separator, newLine));
            }

            try
            {
                File.AppendAllText(file, output.ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
        }
    }
}
