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
        public int mapGen_sizeX = 24;
        public int mapGen_sizeY = 24;
        public double mapGen_proportionOfMapForHumans = 0.65;

        public int mapGen_stepsPerIsland = 12;
        public int mapGen_maxBrushSize = 5;

        public float mapGen_minHabitabilityForHumans = 0.15f;
        public int soc_untitledPeople = 3;
        public int soc_maxUntitledPeople = 7;

        public double econ_multFromBuff = 0.75;//How much to mult/div by if a society rules in favour or against a given economic trait being priviledged/penalised
        public int mapGen_burnInSteps = 0;
        
        public double relObj_defaultLiking = 5;
        public double relObj_decayRate = 0.97;
        public double combat_prestigeLossFromConquest = 0.333;//multiplier on captured lords' prestige
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 1.25;//How much of an army is destroyed in a battle at maximum roll against an equivalent force
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land

        public int war_defaultLength = 10;
        internal double combat_maxMilitaryCapExponent = 0.75;//Used to reduce the power of larger nations
        internal int econ_buffDuration = 50;
        internal float minInformationAvailability = 0.2f;

        internal double utility_econEffect = 0.35;
        internal double utility_econEffectOther = 0.15;
        public double utility_militaryTargetRelStrengthOffensive = 200;
        public double utility_militaryTargetRelStrengthDefensive = 300;
        public double person_prestigeDeltaPerTurn = 2;
        public double person_threatMult = 100;
        internal double person_defaultPrestige = 5;

        public double combat_defensivePostureDmgMult = 0.666;

        internal int society_votingDuration = 2;
        internal double society_votingRelChangePerUtilityPositive = 0.04;//If benefitted by a vote
        internal double society_votingRelChangePerUtilityNegative = 0.08;//If harmed by a vote
        public int society_instablityTillRebellion = 10;
        public int society_rebelLikingThreshold = -5;
        internal int society_zeitDuration = 3;
        internal double society_sovreignPrestige = 10;
        public double society_unlandedTitleUtilityMult = 1;
        public double society_threatMultFromOffensivePosture = 0.5;
        public int society_minTimeBetweenLocReassignments = 40;
        public int society_minTimeBetweenTitleReassignments = 30;
        internal double society_landedTitleUtilityMult = 0.33;
        internal double society_wouldBeOutvotedUtilityMult = 0.25;

        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
