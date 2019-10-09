using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    /**
     * Used to hold all manner of items. Exact semantics of this object is dependent on the exact voting issue itself
     */
    public class VoteOption
    {
        public Person person;
        public Location location;
        public SocialGroup group;
        public EconTrait econ_from;
        public EconTrait econ_to;
        public int index = -1;//For stuff which doesn't have a discrete target, such as "Declare war yes/no"

        public Dictionary<Person, double> randomUtility = new Dictionary<Person, double>();
        public Dictionary<Person, List<ReasonMsg>> msgs = new Dictionary<Person, List<ReasonMsg>>();
        public List<Person> votesFor = new List<Person>();

        public double votingWeight;


        public double getBaseUtility(Person voter)
        {
            if (randomUtility.ContainsKey(voter))
            {
                return randomUtility[voter];
            }
            double u = Eleven.random.NextDouble()*0.0001;
            randomUtility.Add(voter, u);
            return u;
        }
        public override string ToString()
        {
            return info();
        }
        public string info()
        {
            string reply = "";
            if (person != null) { reply += person.getFullName() + " "; }
            if (location != null) { reply += location.getName() + " "; }
            if (group != null) { reply += group.getName() + " "; }
            if (econ_from != null) { reply += econ_from.name + " "; }
            if (econ_to != null) { reply += econ_to.name + " "; }
            if (index != -1) { reply += index + " "; }

            return reply;
        }

        public string fixedLenInfo()
        {
            string line = "";
            VoteOption opt = this;
            if (opt.econ_from != null) { line += " eFrom  " + Eleven.toFixedLen("" + opt.econ_from.name, 12); }
            if (opt.econ_to != null) { line += " eTo    " + Eleven.toFixedLen("" + opt.econ_from.name, 12); }
            if (opt.group != null) { line += " group  " + Eleven.toFixedLen("" + opt.group.getName(), 12); }
            if (opt.location != null) { line += " loc    " + Eleven.toFixedLen("" + opt.location.getName(), 12); }
            if (opt.person != null) { line += " person " + Eleven.toFixedLen("" + opt.person.getFullName(), 12); }
            line += " ind " + opt.index;
            return line;
        }
    }
}
