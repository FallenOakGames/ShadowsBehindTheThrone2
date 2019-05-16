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
        public Title_Land title;
        public double basePrestige = 10;

        public Settlement(Location loc)
        {
            this.location = loc;
        }

        public virtual double getPrestige()
        {
            return basePrestige;
        }

        public List<EconTrait> econTraits()
        {
            return location.hex.province.econTraits;
        }

        public virtual void turnTick()
        {
            if (title != null && title.heldBy != null && title.heldBy.society != this.location.soc)
            {
                World.log("Settlement " + this.name + " recognises loss of title of " + title.heldBy.getFullName());
            }
        }

        public virtual Sprite getSprite()
        {
            return location.map.world.textureStore.loc_green;
        }
    }
}
