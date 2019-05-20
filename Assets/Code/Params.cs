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

        public int burnInSteps = 0;
        
        public double relObj_defaultLiking = 10;
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 0.25;//How much of an army is destroyed in a battle at maximum roll against an equivalent force
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land

        public int war_defaultLength = 10;
        internal double person_defaultPrestige = 5;
        internal int society_votingDuration = 2;
        internal double combat_maxMilitaryCapExponent = 0.75;//Used to reduce the power of larger nations

        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
