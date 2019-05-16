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
        public int sizeX = 24;
        public int sizeY = 24;

        public int stepsPerIsland = 12;
        public int maxBrushSize = 5;

        public float minHabitabilityForHumans = 0.2f;
        public int soc_untitledPeople = 3;
        public int soc_maxUntitledPeople = 7;

        public int burnInSteps = 3;
        
        public double relObj_defaultLiking = 10;

        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
