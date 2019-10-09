﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_SetDefensiveTarget : VoteIssue
    {

        public VoteIssue_SetDefensiveTarget(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            string reply = "SetDefensiveTarget";
            if (society.defensiveTarget == null){
                reply += "(cur:None)";
            }else{
                reply += "(cur:" + society.defensiveTarget.getName() + ")";
            }
            return reply;
        }
        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);
            
            
            double localU = 0;

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
            if (society.defensiveTarget != null)
            {
                localU = 0;
                
                //Add current target threat
                foreach (ThreatItem threat in voter.threatEvaluations)
                {
                    if (threat.group == society.defensiveTarget)
                    {
                        localU -= threat.threat;
                        msgs.Add(new ReasonMsg("Threat of current target (" + society.defensiveTarget.getName() + ")", localU));
                        u += localU;
                        break;
                    }
                }
                
            }
            

            return u;
        }

        public override void implement(VoteOption option)
        {
            society.defensiveTarget = option.group;
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
