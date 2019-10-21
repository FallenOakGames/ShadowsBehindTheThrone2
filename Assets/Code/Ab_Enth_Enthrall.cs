using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_Enth_Enthrall : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            hex.location.person().state = Person.personState.enthralled;
            map.overmind.enthralled = hex.location.person();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled != null) { return false; }

            if (hex.location == null) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (hex.location.settlement.title == null) { return false; }
            if (hex.location.settlement.title.heldBy == null) { return false; }

            if (hex.location.soc == null) { return false; }
            if (hex.location.soc is Society == false) { return false; }

            Society soc = (Society)hex.location.soc;
            return soc.getEnthrallables().Contains(hex.location.settlement.title.heldBy);
        }

        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Enthralls a lower-prestige member of a society.";
        }

        public override string getName()
        {
            return "Enthrall";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}