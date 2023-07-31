using Enhancement0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Enhancement0
{
    internal class Month
    {
        String CustId = "";
        String name = "";
        List<Session> sessions = new List<Session>();
        List<Type> types = new List<Type>();

        public Month() { }

        public Month(string name, string CustId)
        {
            this.name = name;
            this.CustId = CustId;
        }
        public void addsession(Session session)
        {
            this.sessions.Add(session);
            addtypevise(session);
        }

        public void addtypevise(Session session)
        {
            string type = session.getEC2Type();
            bool flag = false;

            for (int i = 0; i < this.types.Count; i++)
            {
                if (types[i].getname().Equals(type))
                {
                    this.types[i].addsession(session);
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                Type typ = new Type(session.getEC2Type());
                typ.addsession(session);

                this.types.Add(typ);
            }
        }


        public string getname()
        {
            return this.name;
        }

        public void gereratebill()
        {
            if (total() != 0.0)
            {
                OutputCSV op = new OutputCSV(this.CustId, this.name, total(), allcharge());
            }
        }

        public List<TypeCharge> allcharge()
        {
            List<TypeCharge> allcharge = new List<TypeCharge>();

            for (int i = 0; i < this.types.Count; i++)
            {
                allcharge.Add(types[i].typecharge());
            }

            return allcharge;
        }

        public double total()
        {
            double total = 0.0;

            for (int i = 0; i < this.types.Count; i++)
            {
                total += this.types[i].billoftype();
            }
            return total;
        }
    }
}
