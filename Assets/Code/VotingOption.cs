using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    /**
     * Used to hold all manner of items. Exact semantics of this object is dependent on the exact voting issue itself
     */
    public class VotingOption
    {
        public Person person;
        public Location location;
        public SocialGroup group;
        public EconTrait econ_from;
        public EconTrait econ_to;

        public Dictionary<Person, List<VoteMsg>> msgs = new Dictionary<Person, List<VoteMsg>>();
        public List<Person> votesFor = new List<Person>();

        public double votingWeight;

        public override string ToString()
        {
            return info();
        }
        public string info()
        {
            string reply = "OPT: ";
            if (person != null) { reply += person.getFullName() + " "; }
            if (location != null) { reply += location.getName() + " "; }
            if (group != null) { reply += group.getName() + " "; }
            if (econ_from != null) { reply += econ_from.name + " "; }
            if (econ_to != null) { reply += econ_to.name + " "; }

            return reply;
        }
    }
}
