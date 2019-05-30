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

        public double econ_multFromBuff = 1.3333;//How much to mult/div by if a society rules in favour or against a given economic trait being priviledged/penalised
        public int burnInSteps = 0;
        
        public double relObj_defaultLiking = 10;
        public double relObj_decayRate = 0.93;
        public double combat_prestigeLossFromConquest = 0.333;//multiplier on captured lords' prestige
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 0.25;//How much of an army is destroyed in a battle at maximum roll against an equivalent force
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land

        public int war_defaultLength = 10;
        internal double person_defaultPrestige = 5;
        internal int society_votingDuration = 2;
        internal double combat_maxMilitaryCapExponent = 0.75;//Used to reduce the power of larger nations
        internal int econ_buffDuration = 15;
        internal double society_votingRelChangePerUtilityPositive = 0.0075;//If benefitted by a vote
        internal double society_votingRelChangePerUtilityNegative = 0.01;//If harmed by a vote
        internal double utility_econEffect = 0.05;
        internal float minInformationAvailability = 0.2f;

        public double utility_militaryTargetRelStrength = 100;
        public double person_prestigeDeltaPerTurn = 2;
        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
