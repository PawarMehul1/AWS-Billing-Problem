using Enhancement0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enhancement0
{
    internal class Type
    {
        string name = "";
        double rate;
        List<Session> sessions = new List<Session>();
        HashSet<string> instances = new HashSet<string>();

        public Type() { }

        public Type(string name)
        {
            this.name = name;
            setrate();
        }

        public void setrate()
        {
            string charge_path = "C:/Users/india/Desktop/Internship/TestCases/Case1/Input/AWSResourceTypes.csv";
            string[] charges = File.ReadAllLines(charge_path);


            for (int i = 0; i < charges.Length; i++)
            {
                string record = charges[i];

                string[] delimetrs = { "\"", ",", " ", "$" };
                String[] values = record.Split(delimetrs, StringSplitOptions.None);

                if (values[4].Equals(this.name))
                {
                    this.rate = double.Parse(values[8]);
                }
            }
        }

        public string getname()
        {
            return this.name;
        }

        public void addsession(Session session)
        {
            this.sessions.Add(session);
            addInstances(session.getEC2instance());
        }

        public void addInstances(String inst)
        {
            this.instances.Add(inst);
        }

        public int Countinstance()
        {
            return this.instances.Count;
        }
        public double billoftype()
        {
            return (double)(billedtime()) * this.rate;

        }

        public int usedseconds()
        {
            int seconds = 0;
            for (int i = 0; i < this.sessions.Count; i++)
            {
                seconds += this.sessions[i].Seconds();
            }

            return seconds;
        }

        public int billedtime()
        {

            int hours = 0;
            for (int i = 0; i < this.sessions.Count; i++)
            {
                int sec = this.sessions[i].Seconds();

                hours += (int)(sec / 3600);

                if (sec % 3600 != 0)
                {
                    hours++;
                }
            }
            return hours;
        }

        public TypeCharge typecharge()
        {
            TypeCharge typcrg = new TypeCharge(this.name, Countinstance(), usedseconds(), billedtime(), this.rate, billoftype());

            return typcrg;
        }
    }
}
