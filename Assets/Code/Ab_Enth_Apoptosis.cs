﻿using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_Enth_Apoptosis : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);


            map.world.prefabStore.popImgMsg(
                "You discard your worthless vessel, " + map.overmind.enthralled.getFullName() + ".",
                map.world.wordStore.lookup("ABILITY_APOPTOSIS"));
            hex.location.person().die("Walked into the night and never returned");
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            return true;

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_apoptosisCost;
        }

        public override string getDesc()
        {
            return "Perhaps your enthralled is not as useful an instrument as you would like. Perhaps their use has come to an end. Remove them, so you may enthrall another."
                + "\n[Requires an enthralled]";
        }

        public override string getName()
        {
            return "Apoptosis";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}