using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_Enth_Enshadow : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            hex.location.person().shadow = 1;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }

            return (hex.location.person().state == Person.personState.enthralled && hex.location.person().shadow < 1);

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_enshadowCost;
        }

        public override string getDesc()
        {
            return "Drapes the enthralled noble in shadow, which they can then spread. The shadow spreads to those who like the enthralled within the society, and spreads faster with higher prestige.";
        }

        public override string getName()
        {
            return "Enshadow Entralled";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}