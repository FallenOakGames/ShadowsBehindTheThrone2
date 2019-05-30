using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Assets.Code
{
    public class VoteIssue_EconomicRebalancing : VoteIssue
    {
        public VoteIssue_EconomicRebalancing(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override double computeUtility(Person p, VotingOption option, List<VoteMsg> msgs)
        {
            double u = 0;

            foreach (Person affected in society.people)
            {
                if (affected.title_land != null)
                {
                    double delta = 1;
                    if (affected.title_land.settlement.econTraits().Contains(option.econ_to)) { delta *= society.map.param.econ_multFromBuff; }
                    if (affected.title_land.settlement.econTraits().Contains(option.econ_from)) { delta /= society.map.param.econ_multFromBuff; }

                    //Run off the base prestige, so all change is regarded the same, regardless of existing changes
                    double localU = delta * p.getRelation(affected).getLiking() * affected.title_land.settlement.basePrestige * society.map.param.utility_econEffect;

                    msgs.Add(new VoteMsg("Prestige change for " + affected.getFullName(), localU));
                    u += localU;
                }
            }

            u += Eleven.random.NextDouble() * 0.00001;

            return u;
        }

        public override void implement(VotingOption option)
        {
            base.implement(option);
            EconEffect effect = new EconEffect(society.map, option.econ_from, option.econ_to);
            World.log(society.getName() + " implements economic policy, moving focus from " + option.econ_from.name + " to " + option.econ_to.name);
            society.econEffects.Add(effect);
        }

        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}