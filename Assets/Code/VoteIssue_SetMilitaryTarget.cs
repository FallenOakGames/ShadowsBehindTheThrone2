using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_SetOffensiveTarget : VoteIssue
    {

        public VoteIssue_SetOffensiveTarget(Society soc,Person proposer) : base(soc,proposer)
        {
        }


        public override string ToString()
        {
            string reply = "SetOffensiveTarget";
            if (society.offensiveTarget == null)
            {
                reply += "(cur:None)";
            }
            else
            {
                reply += "(cur:" + society.offensiveTarget.getName() + ")";
            }
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);
            

            double ourStrength = society.currentMilitary;
            double theirStrength = option.group.currentMilitary;
            double localU = 0;
            
            
            //1 if we're 100% of the balance, -1 if they are
            double relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength);

            localU = society.map.param.utility_militaryTargetRelStrength*relativeStrength;
            msgs.Add(new ReasonMsg("Relative Strength of Current Militaries", localU));
            u += localU;

            foreach (ThreatItem threat in voter.threatEvaluations)
            {
                if (threat.group == option.group)
                {
                    localU = threat.threat;
                    msgs.Add(new ReasonMsg("Perceived Threat", localU));
                    u += localU;
                    break;
                }
            }


            //If we already have a military target
            if (society.offensiveTarget != null)
            {
                theirStrength = society.offensiveTarget.currentMilitary;
                localU = 0;

                //1 if we're 100% of the balance, -1 if they are
                relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength);

                //Add current target threat
                foreach (ThreatItem threat in voter.threatEvaluations)
                {
                    if (threat.group == society.offensiveTarget)
                    {
                        localU -= threat.threat;
                        break;
                    }
                }

                localU -= society.map.param.utility_militaryTargetRelStrength * relativeStrength;
                msgs.Add(new ReasonMsg("Desirability of current target (" + society.offensiveTarget.getName() + ")", localU));
                u += localU;
            }
            

            return u;
        }

        public override void implement(VoteOption option)
        {
            society.offensiveTarget = option.group;
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
