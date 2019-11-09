using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Code
{
    public class Ab_Soc_ShareEvidence : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            double spaceLeft = 1 - hex.location.person().evidence;
            double toShare = map.overmind.enthralled.evidence * map.param.ability_shareEvidencePercentage;
            double shared = Math.Min(spaceLeft, toShare);
            map.overmind.enthralled.evidence -= shared;
            hex.location.person().evidence += shared;
            hex.location.person().getRelation(map.overmind.enthralled).addLiking(-World.staticMap.param.ability_shareEvidenceLikingCost, "Asked to receive evidence of dubious nature", map.turn);

            map.world.prefabStore.popImgMsg(
                "You transfer evidence from your enthralled, " + map.overmind.enthralled.getFullName() + " to " + hex.location.person().getFullName() + "."
                + " " + (int)(0.5+(100*shared)) + " evidence transferred",
                map.world.wordStore.lookup("SOC_TRANSFER_EVIDENCE"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (map.overmind.enthralled == null) { return false; }
            if (hex.location.person() == map.overmind.enthralled) { return false; }
            if (hex.location.soc != map.overmind.enthralled.society) { return false; }
            if (hex.location.person().getRelation(map.overmind.enthralled).getLiking() < World.staticMap.param.ability_shareEvidenceLikingCost) { return false; }

            return true;
        }

        public override string specialCost()
        {
            return "Cost: -" + World.staticMap.param.ability_shareEvidenceLikingCost + " liking";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Requests another noble to take up to " + (int)(World.staticMap.param.ability_shareEvidencePercentage*100) + "% of the enthralled's evidence, as a favour."
                + "\n[Requires a noble in your enthralled's society with a positive attitude towards you enthralled]";
        }

        public override string getName()
        {
            return "Share Evidence";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}