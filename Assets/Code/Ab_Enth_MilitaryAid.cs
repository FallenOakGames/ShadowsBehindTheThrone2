using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_Enth_MiliaryAid : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Property.addProperty(map, hex.location, "Military Aid");
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.map.overmind.enthralled == null) { return false; }
            if (hex.location == null) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return false; }
            if (hex.location.person() != null && hex.location.person().state == Person.personState.enthralled) { return false; }

            return true;

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_militaryAidCost;
        }

        public override string getDesc()
        {
            return "Supplies a location not held by an enthralled noble with weapons and supplies, granting it +" + World.staticMap.param.ability_militaryAidAmount + " military cap for " + World.staticMap.param.ability_militaryAidDur + " turns."
                + " Useful for creating civil wars by arming dissidents."
                + "\n[Requires a non-enthralled noble, and for you to have an enthralled in existence]";
        }

        public override string getName()
        {
            return "Covert Military Aid";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_shield;
        }
    }
}