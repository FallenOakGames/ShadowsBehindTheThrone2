using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public abstract class VoteIssue
    {
        public Person proposer;
        public Society society;
        public VoteIssue(Society society,Person proposer)
        {
            this.society = society;
            this.proposer = proposer;
        }
        public List<VotingOption> options = new List<VotingOption>();
        public abstract double computeUtility(Person p,VotingOption option, List<VoteMsg> msgs);
        public virtual void implement(VotingOption option)
        {
            //Everyone affected/concerned about the vote now changes their opinion of all the voters for the winning option
            //depending on how much they care and how much they were affected
            foreach (Person p in society.people)
            {
                double deltaRel = computeUtility(p, option, new List<VoteMsg>());
                if (deltaRel > 0)
                {
                    deltaRel *= p.map.param.society_votingRelChangePerUtilityPositive;
                }
                else
                {
                    deltaRel *= p.map.param.society_votingRelChangePerUtilityNegative;
                }
                foreach (Person voter in option.votesFor)
                {
                    p.getRelation(voter).addLiking(deltaRel);
                }
            }
        }
        public abstract bool stillValid(Map map);
    }
}
