using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class SG_GenericDark : SocialGroup
    {
        public SG_GenericDark(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            color = new Color(
                (float)Eleven.random.NextDouble()* colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            color2 = new Color(
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            this.setName("Darkness in " + startingLocation.shortName);

            startingLocation.soc = this;
        }

        public override void turnTick()
        {
            base.turnTick();
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    loc.hex.purity = 0;
                }
            }

            if (Eleven.random.NextDouble() < 0.25)
            {
                Location expansion = null;
                int c = 0;
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this)
                    {
                        foreach (Location l2 in loc.getNeighbours())
                        {
                            if (l2.soc == null)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    expansion = l2;
                                }
                            }
                        }
                    }
                }
                if (expansion != null)
                {
                    expansion.soc = this;
                }
            }
        }
    }
}