using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_Over_InformationBlackout : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Property.addProperty(map, hex.location, "Information Blackout");
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }

            return true;

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_informationBlackoutCost;
        }

        public override string getDesc()
        {
            return "Reduces the flow of information through a location. This causes nobles to fear social groups on the other side the blackout to a lesser extent (see information % on threat breakdown)."
                + "\n[Requires a location]";
        }

        public override string getName()
        {
            return "Information Blackout";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}