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
        public int mapGen_sizeX = 32;
        public int mapGen_sizeY = 24;
        public double mapGen_proportionOfMapForHumans = 0.65;
        public float mapGen_minHabitabilityForHumans = 0.15f;
        public int mapGen_stepsPerIsland = 12;
        public int mapGen_maxBrushSize = 5;
        public int mapGen_burnInSteps = 0;

        public int overmind_maxPower = 24;
        public float overmind_powerRegen = 2;

        public double econ_multFromBuff = 0.75;//How much to mult/div by if a society rules in favour or against a given economic trait being priviledged/penalised
        
        public double relObj_defaultLiking = 5;
        public double relObj_decayRate = 0.98;

        public double combat_prestigeLossFromConquest = 0.333;//multiplier on captured lords' prestige
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 1.25;//How much of an army is destroyed in a battle at maximum roll against an equivalent force
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land
        internal double combat_maxMilitaryCapExponent = 0.75;//Used to reduce the power of larger nations
        public double combat_defensivePostureDmgMult = 0.666;

        public int war_defaultLength = 10;
        internal float minInformationAvailability = 0.2f;

        internal double utility_econEffect = 0.5;
        internal double utility_econEffectOther = 0.25;
        public double utility_militaryTargetRelStrengthOffensive = 200;
        public double utility_militaryTargetRelStrengthDefensive = 300;
        internal double utility_vassaliseReluctance = -100;
        internal double utility_vassaliseMilMult = 100;
        internal double utility_vassaliseThreatMult = 0.5;
        internal double utility_introversionFromInstability = 200;
        internal double utility_militarism = 50;
        public double utility_unlandedTitleMult = 0.5;

        public double person_prestigeDeltaPerTurn = 2;
        public double person_threatMult = 100;
        internal double person_defaultPrestige = 5;

        internal int econ_buffDuration = 50;

        public int soc_untitledPeople = 3;
        public int soc_maxUntitledPeople = 7;
        internal int society_votingDuration = 2;
        internal double society_votingRelChangePerUtilityPositive = 0.04;//If benefitted by a vote
        internal double society_votingRelChangePerUtilityNegative = 0.1;//If harmed by a vote
        public int society_instablityTillRebellion = 10;
        public int society_rebelLikingThreshold = -5;
        internal int society_zeitDuration = 3;
        internal double society_sovreignPrestige = 10;
        internal double society_chancellorPrestige = 10;
        public double society_threatMultFromOffensivePosture = 0.5;
        public int society_minTimeBetweenLocReassignments = 40;
        public int society_minTimeBetweenTitleReassignments = 30;
        internal double society_landedTitleUtilityMult = 0.33;
        internal double society_wouldBeOutvotedUtilityMult = 0.25;

        public double ability_growFleshThreatAdd = 5;
        internal double temporaryThreatDecay = 0.95;
        internal double threat_takeLocation = 3;

        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
