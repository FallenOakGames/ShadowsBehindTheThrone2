using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Code
{
    public class Ab_Over_CancelVote : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = (Society)hex.location.soc;
            soc.voteSession = null;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }



            Society soc = (Society)hex.location.soc;
            if (soc.voteSession == null) { return false; }

            return true;
        }
        
        public override int getCost()
        {
            return World.staticMap.param.ability_cancelVoteCost;
        }

        public override string getDesc()
        {
            return "Cancels an ongoing issue in a society, preventing its effects from occuring (including liking changes). The same issue may, sadly, be re-proposed in the next cycle by the next vote-proposer."
                + "\n[Requires society which is voting]";
        }

        public override string getName()
        {
            return "Cancel Vote";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}