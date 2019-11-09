using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Code
{
    public class Ab_Soc_DenounceOther : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = (Society)hex.location.soc;
            Person victim = hex.location.person();

            victim.getRelation(map.overmind.enthralled).addLiking(-100, "Denounced me", map.turn);
            foreach (Person p in soc.people)
            {
                if (p == victim) { continue; }
                if (p == map.overmind.enthralled) { continue; }

                RelObj relEnth = p.getRelation(map.overmind.enthralled);
                RelObj relVic = p.getRelation(victim);
                if (relVic.suspicion < p.evidence)
                {
                    double gain = victim.evidence - relVic.suspicion;
                    gain = Math.Max(0, gain);gain = Math.Min(1, gain);
                    //1 if you're revealling a fully evil person
                    gain = Math.Pow(gain, 0.5);//Bring it closer to 1, get your money's worth

                    relEnth.suspicion *= (1 - gain);//Suspicion drops to 0 if you give up a 100% evidence person with no suspicion on them
                    relVic.suspicion += gain;
                    if (relVic.suspicion > 1) { relVic.suspicion = 1; }
                    relEnth.addLiking(gain * 25, "Revealled evidence of darkness", map.turn);
                    World.log(p.getFullName() + " gain " + gain);
                }
            }


            map.world.prefabStore.popImgMsg(
                "You denounce another, unexpectedly revealing the evidence of evil which they hold to the world. " +
                " By denouncing a villain, the suspicion towards your enthralled is reduced.",
                map.world.wordStore.lookup("SOC_DENOUNCE_OTHER"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (map.overmind.enthralled == null) { return false; }
            if (hex.location.person() == map.overmind.enthralled) { return false; }
            if (hex.location.soc != map.overmind.enthralled.society) { return false; }
            if (hex.location.person().evidence == 0) { return false; }

            return true;
        }

        public override string specialCost()
        {
            return "Cost: -100 liking (victim)";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Reveals publicly and accuses the evidence on a noble. Others lose suspicion of your enthralled based on how much suspicion they now gain towards the accused."
                + "\n[Requires a noble with evidence on them in your enthralled's society]";
        }

        public override string getName()
        {
            return "Denounce other";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}