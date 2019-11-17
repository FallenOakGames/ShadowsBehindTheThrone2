using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Code
{
    public class Ab_Over_SowDissent : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            
            other.getRelation(other.society.getSovreign()).addLiking(map.param.ability_sowDissentLikingChange, "Dissent sown", map.turn);

        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (hex.location.person().state == Person.personState.enthralled) { return false; }
            if (hex.location.person().society.getSovreign() == null) { return false; }
            if (hex.location.person() == hex.location.person().society.getSovreign()) { return false; }
            return true;
        }

        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return World.staticMap.param.ability_sowDissentCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_fearmongerCooldown;
        }
        public override string getDesc()
        {
            return "Causes a noble to gain " + World.staticMap.param.ability_sowDissentLikingChange + " disliking for their sovreign."
                + "\n[Requires a noble with a sovreign]";
        }

        public override string getName()
        {
            return "Sow Dissent";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}