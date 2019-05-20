using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public abstract class VoteIssue
    {
        public Society society;
        public VoteIssue(Society society)
        {
            this.society = society;
        }
        public List<VotingOption> options = new List<VotingOption>();
        public abstract double computeUtility(Person p,VotingOption option, List<VoteMsg> msgs);
        public abstract void implement(VotingOption option);
        public abstract bool stillValid(Map map);
    }
}
