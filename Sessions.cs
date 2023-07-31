using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enhancement0
{
    internal class Session
    {
        string custId = "";
        string EC2instance = "";
        string EC2type = "";
        string from = "";
        string until = "";
        public Session() { }

        public Session(string custId, string eC2instance, string eC2type, string from, string until)
        {
            this.custId = custId;
            this.EC2instance = eC2instance;
            this.EC2type = eC2type;
            this.from = from;
            this.until = until;
        }

        public string getcustId()
        {
            return this.custId;

        }
        public string getEC2instance()
        {
            return this.EC2instance;
        }

        public string getEC2Type()
        {
            return this.EC2type;
        }
        public string getfrom()
        {
            return this.from;
        }

        public string getuntil()
        {
            return this.until;
        }
        public int Seconds()
        {
            string[] delimetrs = { ":", "-", "T" };
            String[] start = this.from.Split(delimetrs, StringSplitOptions.None);
            String[] end = this.until.Split(delimetrs, StringSplitOptions.None);

            int[] Day1 = new int[6];
            int[] Day2 = new int[6];

            for (int i = 0; i < 6; i++)
            {
                Day1[i] = int.Parse(start[i]);
                Day2[i] = int.Parse(end[i]);
            }

            DateTime date1 = new DateTime(Day1[0], Day1[1], Day1[2], Day1[3], Day1[4], Day1[5]);
            DateTime date2 = new DateTime(Day2[0], Day2[1], Day2[2], Day2[3], Day2[4], Day2[5]);

            TimeSpan ts = date2 - date1;

            int seconds = (int)(ts.TotalSeconds);

            return seconds;
        }

        public void printsession()
        {
            Console.WriteLine(this.custId + " " + this.EC2instance + " " + this.EC2type + " " + this.from + " " + this.until);
        }
    }
}
