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

            map.world.prefabStore.popImgMsg(
                "You enthrall " + map.overmind.enthralled.getFullName() + ". They are now, until they die, your instrument in this world. Their votes are guided by your hand, and they will"
                + " act as you command within their society.",
                map.world.wordStore.lookup("ABILITY_ENTHRALL"));
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
            double minPrestige = 1000000;
            foreach (Person p in soc.people){
                if (p.title_land == null) { continue; }
                if (p.prestige < minPrestige){
                    minPrestige = p.prestige;
                }
            }
            
            //return soc.getEnthrallables().Contains(hex.location.settlement.title.heldBy);
            return hex.location.person().prestige < (1+minPrestige);
        }

        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Enthralls a lower-prestige member of a society."
                + "\n[Only certain low ranked nobles can be enthralled. You may only have one enthralled at a time]";
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