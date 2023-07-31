using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enhancement0
{
    internal class TypeCharge
    {
        string Resotype = "";
        int totalinstace;
        int totalUsed;
        int totalbilledhr;
        double rate;
        double totalamount;

        public TypeCharge(string Resotype, int totlin, int totuse, int billhr, double rate, double totamount)
        {
            this.Resotype = Resotype;
            this.totalinstace = totlin;
            this.rate = rate;
            this.totalUsed = totuse;
            this.totalbilledhr = billhr;
            this.totalamount = totamount;
        }

        public string mintohr(int min)
        {
            string str = "";
            str += (min / 60).ToString() + ":";

            if ((min % 60) < 10)
                str += "0";

            str += (min % 60).ToString() + ":00";

            return str;
        }

        public string sectohr(int sec)
        {
            string hr = "";

            hr += (sec / 3600).ToString() + ":";
            sec %= 3600;

            if ((sec / 60) < 10)
                hr += "0";

            hr += (sec / 60).ToString() + ":";
            sec %= 60;

            if (sec < 10)
                hr += "0";
            hr += sec.ToString();

            return hr;
        }

        public string[] deatils()
        {
            string[] str = new string[6];

            str[0] = this.Resotype;
            str[1] = this.totalinstace.ToString();
            str[2] = sectohr(this.totalUsed);
            str[3] = mintohr(this.totalbilledhr * 60);
            str[4] = "$" + this.rate.ToString();
            str[5] = "$" + Math.Round(this.totalamount, 4).ToString();

            return str;
        }
    }
}
