using Enhancement0;

class Program
{
    static void Main(string[] args)
    {
        string usage_path = "C:/Users/india/Desktop/Internship/TestCases/Case1/Input/AWSCustomerUsage.csv";
        string[] usage = File.ReadAllLines(usage_path);

        List<Users> allusers = new List<Users>();

        for (int i = 1; i < usage.Length; i++)
        {
            string record = usage[i];

            string[] delimetrs = { "\"", ",", " " };
            String[] values = record.Split(delimetrs, StringSplitOptions.None);

            Session session = new Session(values[4], values[7], values[10], values[13], values[16]);

            bool flag = false;
            for (int j = 0; j < allusers.Count; j++)
            {
                Users cust = allusers[j];

                if (cust.getuserId().Equals(values[4]))
                {
                    cust.addsessions(session);
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                Users cust = new Users(values[4]);
                cust.addsessions(session);

                allusers.Add(cust);
            }

        }


        for (int i = 0; i < allusers.Count; i++)
        {
            Users cust = allusers[i];

            List<Month> months = cust.getmonths();

            for (int j = 0; j < months.Count; j++)
            {
                Month mon = months[j];

                mon.gereratebill();

            }
        }
        Console.WriteLine("Output files are generated Successfully.");
    }
}