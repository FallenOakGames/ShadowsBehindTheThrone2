using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Settlement
    {
        public Location location;
        public string name = "defaultSettlementName";
        public float purity = 1;

        public virtual void turnTick()
        {

        }

        public virtual Sprite getSprite()
        {
            return location.map.world.textureStore.loc_green;
        }
    }
}
