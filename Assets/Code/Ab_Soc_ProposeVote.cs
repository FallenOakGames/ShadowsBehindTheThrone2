using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Code
{
    public class Ab_Soc_Popose_Vote : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = map.overmind.enthralled.society;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled.society.voteSession != null) { return false; }

            return true;
        }

        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Proposes a vote to the society, allowing you to get the society to choose to act in a certain way (if you can convince enough of the voters)."
                + "\n[Requires an enthralled noble and a society not currently voting]";
        }

        public override string getName()
        {
            return "Propose Vote";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}