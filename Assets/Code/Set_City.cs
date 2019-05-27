﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_City : Settlement
    {
        public Set_City(Location loc) : base(loc)
        {
            title = new Title_Land("Count", "Countess",this);
            name = "City of " + loc.shortName;
            basePrestige = 25;
            militaryCapAdd += 10;
            militaryRegenAdd = 1;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_city_roman;
        }
    }
}