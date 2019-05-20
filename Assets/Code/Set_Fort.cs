using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Fort : Settlement
    {
        public Set_Fort(Location loc) : base(loc)
        {
            title = new Title_Land("Baron", "Baroness",this);
            int q = Eleven.random.Next(2);
            if (q == 0)
            {
                name = loc.shortName + " Castle";
            }else if (q == 1)
            {
                name = "Fort " + loc.shortName;
            }


            militaryCapAdd += 5;
            militaryRegenAdd = 1;
            defensiveStrength = 0.5;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_fort;
        }
    }
}
