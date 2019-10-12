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

        public override double computeUtility(Person p, VoteOption option, List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(p);

            double advtangeToMe = 0;
            double advantageToAllies = 0;
            double advantageToEnemeies = 0;
            foreach (Person affected in society.people)
            {
                if (affected.title_land != null)
                {
                    double delta = 1;
                    if (affected.title_land.settlement.econTraits().Contains(option.econ_to)) { delta *= society.map.param.econ_multFromBuff; }
                    if (affected.title_land.settlement.econTraits().Contains(option.econ_from)) { delta /= society.map.param.econ_multFromBuff; }

                    delta = 1 - delta;
                    //Run off the base prestige, so all change is regarded the same, regardless of existing changes
                    double localU = 0;
                    if (affected == p)
                    {
                        advtangeToMe = delta * p.getRelation(affected).getLiking() * affected.title_land.settlement.basePrestige * society.map.param.utility_econEffect;
                    }
                    else
                    {
                        if (p.getRelation(affected).getLiking() > 0)
                        {
                            advantageToAllies += delta * p.getRelation(affected).getLiking() * affected.title_land.settlement.basePrestige * society.map.param.utility_econEffectOther;
                        }
                        else
                        {
                            advantageToEnemeies += delta * p.getRelation(affected).getLiking() * affected.title_land.settlement.basePrestige * society.map.param.utility_econEffectOther;
                        }
                    }
                }
            }

            msgs.Add(new ReasonMsg("Advantage to me", advtangeToMe));
            u += advtangeToMe;
            msgs.Add(new ReasonMsg("Advantage to allies", advantageToAllies));
            u += advantageToAllies;
            msgs.Add(new ReasonMsg("Advantage to enemies", advantageToEnemeies));
            u += advantageToEnemeies;

            //World.log("Econ advantages " + advtangeToMe + " " + advantageToAllies + " " + advantageToEnemeies);

            return u;
        }

        public override void implement(VoteOption option)
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