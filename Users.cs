using Enhancement0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Enhancement0
{
    internal class Users
    {
        string userId = "";
        string username = "";
        List<Session> sessions = new List<Session>();
        List<Month> months = new List<Month>();

        public Users() { }

        public Users(string userId)
        {
            this.userId = userId;
        }

        public void addsessions(Session session)
        {
            this.sessions.Add(session);

            monthlysessions(session);
        }

        public string getuserId()
        {
            return this.userId;
        }

        public void addsessioninmonth(Session session, string name)
        {
            bool flag = false;
            for (int j = 0; j < this.months.Count; j++)
            {
                if (this.months[j].getname().Equals(name))
                {
                    this.months[j].addsession(session);
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                Month mon = new Month(name, this.userId);
                mon.addsession(session);

                this.months.Add(mon);
            }
        }

        public void monthlysessions(Session session)
        {
            string from = session.getfrom();
            string until = session.getuntil();

            string[] delimetrs = { ":", "-", "T" };
            String[] start = from.Split(delimetrs, StringSplitOptions.None);
            String[] end = until.Split(delimetrs, StringSplitOptions.None);

            bool flag = (int.Parse(start[0]) + 1 == int.Parse(end[0]) && end[1].Equals("01") && end[2].Equals("01") && end[3].Equals("00") && end[4].Equals("00") && end[5].Equals("00"));

            if (start[0].Equals(end[0]) && start[1].Equals(end[1]))
            {
                string name = start[0] + "-" + start[1];

                addsessioninmonth(session, name);
            }

            else if (start[0].Equals(end[0]) && (!start[1].Equals(end[1])) || flag)
            {
                int year = int.Parse(start[0]);

                int ms = int.Parse(start[1]);
                int me = int.Parse(end[1]);
                if (int.Parse(start[0]) + 1 == int.Parse(end[0]))
                {
                    me = 12;
                }

                for (int i = ms; i <= me; i++)
                {
                    string name = i.ToString();
                    if (name.Length == 1)
                        name = "0" + name;

                    name = start[0] + "-" + name;

                    string name2 = "";
                    string new_until = "";

                    if (i != me)
                    {
                        name2 = (i + 1).ToString();
                        if (name2.Length == 1)
                            name2 = "0" + name2;

                        name2 = start[0] + "-" + name2;
                        new_until = name2 + "-01T00:00:00";
                    }

                    string new_from = name + "-01T00:00:00";

                    if (i == ms)
                        new_from = from;
                    if (i == me)
                        new_until = until;
                    Session ss = new Session(session.getcustId(), session.getEC2instance(), session.getEC2Type(), new_from, new_until);

                    addsessioninmonth(ss, name);
                }
            }
            else
            {
                int year1 = int.Parse(start[0]);
                int year2 = int.Parse(end[0]);

                for (int i = year1; i <= year2; i++)
                {
                    string new_from = i.ToString();
                    new_from += "-01-01T00:00:00";

                    string new_until = (i + 1).ToString();
                    new_until += "-01-01T00:00:00";

                    if (i == year1)
                        new_from = from;

                    if (i == year2)
                        new_until = until;
                    Session subSession = new Session(session.getcustId(), session.getEC2instance(), session.getEC2Type(), new_from, new_until);

                    monthlysessions(subSession);
                }
            }
        }

        public List<Month> getmonths()
        {
            return this.months;
        }
    }
}
