using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_Fishman_Lair : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }


            SG_Fishmen soc = null;
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg is SG_Fishmen)
                {
                    soc = (SG_Fishmen)sg;
                }
            }
            if (soc == null)
            {
                map.socialGroups.Add(new SG_Fishmen(map, hex.location));
            }
            else
            {
                hex.location.soc = soc;
            }
            
            hex.location.settlement = new Set_Fishman_Lair(hex.location);
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc != null) { return false; }
            if (hex.location.settlement != null) { return false; }
            if (!hex.location.isOcean) { return false; }
            return true;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fishmanLairCost;
        }

        public override string getDesc()
        {
            return "Creates a fishman lair. This creates or expands your fishman civilisation. Fishmen are covert and difficult to notice until revealed by taking action, but must raid to build population."
                + "\n[Requires an empty ocean location]";
        }

        public override string getName()
        {
            return "Establish Fishman Lair";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fishman;
        }
    }
}