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
        public List<VoteOption> options = new List<VoteOption>();
        public abstract double computeUtility(Person p,VoteOption option, List<ReasonMsg> msgs);
        public virtual void implement(VoteOption option)
        {
        }
        public abstract bool stillValid(Map map);
        
        public void changeLikingForVotes(VoteOption option)
        {
            //Everyone affected/concerned about the vote now changes their opinion of all the voters for the winning option
            //depending on how much they care and how much they were affected
            foreach (Person p in society.people)
            {
                double utility = computeUtility(p, option, new List<ReasonMsg>());

                double deltaRel = utility;
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
                    p.getRelation(voter).addLiking(deltaRel,"Vote on issue " + this.ToString(),society.map.turn);
                }
            }
        }
    }
}
