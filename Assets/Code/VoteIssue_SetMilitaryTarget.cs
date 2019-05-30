﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_SetOffensiveTarget : VoteIssue
    {

        public VoteIssue_SetOffensiveTarget(Society soc) : base(soc)
        {
        }

        public override double computeUtility(Person voter, VotingOption option,List<VoteMsg> msgs)
        {
            double u = 0;

            double ourStrength = society.currentMilitary;
            double theirStrength = option.group.currentMilitary;
            double localU = 0;

            if (ourStrength < 1)
            {
                localU = -1000;
                msgs.Add(new VoteMsg("We are too weak", localU));
                u += localU;
                return u;
            }

            //1 if we're 100% of the balance, -1 if they are
            double relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength);

            localU = society.map.param.utility_militaryTargetRelStrength*relativeStrength;
            msgs.Add(new VoteMsg("Relative strength of current militaries", localU));
            u += localU;


            //If we already have a military target
            if (society.offensiveTarget != null)
            {
                theirStrength = society.offensiveTarget.currentMilitary;
                localU = 0;

                //1 if we're 100% of the balance, -1 if they are
                relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength);

                localU = -society.map.param.utility_militaryTargetRelStrength * relativeStrength;
                msgs.Add(new VoteMsg("Desirability of current target (" + society.offensiveTarget.getName() + ")", localU));
                u += localU;
            }


            u += Eleven.random.NextDouble() * 0.001;//Noise to introduce randomness

            return u;
        }

        public override void implement(VotingOption option)
        {
            society.offensiveTarget = option.group;
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
