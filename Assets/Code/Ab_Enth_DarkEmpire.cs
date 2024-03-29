﻿using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_Enth_DarkEmpire : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            map.world.prefabStore.popImgMsg(
                map.overmind.enthralled.society.getName() + " turns fully against the light, casting off what was left of their disguise. The nations of men tremble at this threat.",
                map.world.wordStore.lookup("ABILITY_DARK_EMPIRE"));

            map.overmind.enthralled.society.isDarkEmpire = true;
            map.overmind.enthralled.society.threat_mult += map.param.ability_darkEmpireThreatMultGain;

        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled.society.getSovreign() != map.overmind.enthralled) { return false; }
            if (map.overmind.enthralled.society.isDarkEmpire) { return false; }
            return true;

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_apoptosisCost;
        }

        public override string getDesc()
        {
            return "Declares your society for the dark, devoting it to the cause. All nobles will begin to gain shadow per turn, regardless of liking. Your nation will have higher threat to other nobles."
                + "\n[Requires an enthralled which is sovreign of a society]";
        }

        public override string getName()
        {
            return "Dark Empire";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}