using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_DeclareWar : VoteIssue
    {
        public SocialGroup target;

        public VoteIssue_DeclareWar(Society soc,SocialGroup target,Person proposer) : base(soc,proposer)
        {
            this.target = target;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            //Option 0 is "Don't declare war"
            //Multiply values based on this, as they should be symmetric for most parts
            double parityMult = 1;
            if (option.index == 0) { parityMult = -1; }

            double ourStrength = society.currentMilitary;
            double theirStrength = target.currentMilitary;
            if (ourStrength + theirStrength == 0) { ourStrength += 0.001; }
            double localU = 0;

            if (ourStrength < 1)
            {
                localU = -1000*parityMult;
                msgs.Add(new ReasonMsg("We are too weak", localU));
                u += localU;
                return u;
            }

            //1 if we're 100% of the balance, -1 if they are
            double relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength);

            localU = society.map.param.utility_militaryTargetRelStrength*relativeStrength * parityMult;
            msgs.Add(new ReasonMsg("Relative strength of current militaries", localU));
            u += localU;

            localU = voter.politics_militarism * parityMult;
            msgs.Add(new ReasonMsg("Militarism of " + voter.getFullName(), localU));
            u += localU;

            if (this.society.defensiveTarget != null && this.society.defensiveTarget != this.society.offensiveTarget)
            {

                theirStrength = society.defensiveTarget.currentMilitary;
                //0 if we're 100% of the balance, -1 if they are
                relativeStrength = (((ourStrength - theirStrength) / (ourStrength + theirStrength))-1)/2;

                localU = society.map.param.utility_militaryTargetRelStrength * relativeStrength * parityMult;
                msgs.Add(new ReasonMsg("Defensive Target's Capabilities", localU));
                u += localU;
            }
            

            return u;
        }

        public override void implement(VoteOption option)
        {
            society.map.declareWar(society, target);
        }
        public override bool stillValid(Map map)
        {
            if (society.offensiveTarget == null) { return false; }
            if (society.offensiveTarget != target) { return false; }
            if (society.map.socialGroups.Contains(target) == false) { return false; }
            return true;
        }
    }
}
