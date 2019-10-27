using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Code
{
    public class Ab_Fishman_Raid : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            Settlement raidTarget = null;
            SG_Fishmen receiver = null;
            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc is Society && l.settlement != null)
                {
                    raidTarget = l.settlement;
                }
                if (l.soc is SG_Fishmen)
                {
                    receiver = (SG_Fishmen)l.soc;
                }
            }
            receiver.currentMilitary = Math.Min(receiver.currentMilitary + map.param.ability_fishmanRaidMilAdd, receiver.maxMilitary);
            receiver.temporaryThreat += map.param.ability_fishmanRaidTemporaryThreat;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (!(hex.location.soc is Society)) { return false; }
            if (hex.location.settlement == null) { return false; }

            foreach (Location l in hex.location.getNeighbours())
            {
                if (l.soc != null && (l.soc is SG_Fishmen))
                {
                    return true;
                }
            }
            return false;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fishmanRaidCost;
        }

        public override string getDesc()
        {
            return "Fishmen raiders rise from the deep to drag away captives to turn into fishmen soldiers. Adds " + World.staticMap.param.ability_fishmanRaidMilAdd + " military, but adds "
                + World.staticMap.param.ability_fishmanRaidTemporaryThreat + " temporary threat."
                 + "\n[Requires a human settlement bordering a fishman-held location]";
        }

        public override string getName()
        {
            return "Raid the Coasts";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}