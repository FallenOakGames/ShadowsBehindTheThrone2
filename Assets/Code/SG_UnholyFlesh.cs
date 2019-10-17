using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class SG_UnholyFlesh : SocialGroup
    {
        public SG_UnholyFlesh(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            color = new Color(
                (float)(Eleven.random.NextDouble()* colourReducer) + (1-colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            color2 = new Color(
                (float)(Eleven.random.NextDouble() * colourReducer) + (1 - colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            this.setName("Unholy Flesh from " + startingLocation.shortName);

            startingLocation.soc = this;
            startingLocation.settlement = new Set_UnholyFlesh_Seed(startingLocation);
            this.threat_mult = 2;
        }

        public override void turnTick()
        {
            base.turnTick();
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    if (loc.settlement == null)
                    {
                        loc.soc = null;
                    }
                    else
                    {
                        loc.hex.purity = 0;
                    }
                }
            }
        }

        public override void takeLocationFromOther(SocialGroup def, Location taken)
        {
            base.takeLocationFromOther(def, taken);
            
            if (taken.settlement != null)
            {
                taken.settlement = new Set_UnholyFlesh_Ganglion(taken);
            }
        }
    }
}