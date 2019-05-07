using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    /*
     * Chuck any parameter values in here, to be referenced via world
     */
    //[Serializable,HideInInspector]
    public class Params
    {
        public int sizeX = 32;
        public int sizeY = 32;

        public int stepsPerIsland = 12;
        public int maxBrushSize = 5;

        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
