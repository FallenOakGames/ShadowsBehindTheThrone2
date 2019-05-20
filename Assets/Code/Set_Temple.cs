using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Abbey : Settlement
    {
        public Set_Abbey(Location loc) : base(loc)
        {
            title = new Title_Land("Abott", "Abess");
            int q = Eleven.random.Next(3);
            if (q == 0)
            {
                name = loc.shortName + " Abbey";
            }else if (q == 1)
            {
                name = loc.shortName + " Cathedral";
            }
            else if (q == 2)
            {
                name = "Church of " + loc.shortName;
            }


            militaryCapAdd += 5;
            militaryRegenAdd = 2;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_church;
        }
    }
}
