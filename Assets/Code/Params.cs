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
        public float overmind_powerRegen = 1.5f;
        internal bool overmind_singleAbilityPerTurn = true;

        public double econ_multFromBuff = 0.75;//How much to mult/div by if a society rules in favour or against a given economic trait being priviledged/penalised
        
        public double relObj_defaultLiking = 5;
        public double relObj_decayRate = 0.98;

        public double combat_prestigeLossFromConquest = 0.333;//multiplier on captured lords' prestige
        public double combat_thresholdAttackStrength = 0.25;
        public double combat_lethality = 0.8;//How much of an army is destroyed in a battle at maximum roll against an equivalent force
        public double combat_takeLandThreshold = 1.25;//How much over their strength do you have to be to take some land
        internal double combat_maxMilitaryCapExponent = 0.75;//Used to reduce the power of larger nations
        public double combat_defensivePostureDmgMult = 0.666;

        public int war_defaultLength = 8;
        internal float minInformationAvailability = 0.2f;

        internal double utility_econEffect = 0.5;
        internal double utility_econEffectOther = 0.20;
        public double utility_militaryTargetRelStrengthOffensive = 250;
        public double utility_militaryTargetRelStrengthDefensive = 300;
        internal double utility_vassaliseReluctance = -100;
        internal double utility_vassaliseMilMult = 80;
        internal double utility_vassaliseThreatMult = 0.75;
        internal double utility_introversionFromInstability = 50;
        internal double utility_militarism = 100;
        internal double utility_landedTitleMult = 0.1;
        public double utility_unlandedTitleMult = 0.1;
        public double utility_introversionFromSuspicion = 3;
        public double utility_killSuspectFromSuspicion = 350;
        internal double utility_killSuspectRelucatance = 66;
        internal double utility_wouldBeOutvotedMult = 0.25;
        internal double utility_landedTitleAssignBaseline = 100;
        public double utility_prestigeMultForTitle = 1.5;

        public double person_maxLikingGainFromVoteAccordance = 30;
        public double person_maxLikingLossFromVoteDiscord = -50;
        public double person_prestigeDeltaPerTurn = 2;
        public double person_threatMult = 100;
        internal double person_defaultPrestige = 5;
        internal double person_evidencePerShadow = 0.025;
        internal double person_suspicionPerEvidence = 0.075;
        internal double person_dislikingFromSuspicion = -200;
        internal double person_shadowContagionMult = 0.05;
        internal double person_threatFromSuspicion = 400;
        internal double person_shadowDecayPerTurn = 0.005;

        internal int econ_buffDuration = 50;

        public int soc_untitledPeople = 3;
        public int soc_maxUntitledPeople = 7;
        internal int society_votingDuration = 2;
        internal double society_votingRelChangePerUtilityPositive = 0.06;//If benefitted by a vote
        internal double society_votingRelChangePerUtilityNegative = 0.13;//If harmed by a vote
        public int society_instablityTillRebellion = 10;
        public int society_rebelLikingThreshold = -5;
        internal int society_zeitDuration = 3;
        internal double society_sovreignPrestige = 10;
        public double society_threatMultFromOffensivePosture = 0.5;
        public int society_minTimeBetweenLocReassignments = 40;
        public int society_minTimeBetweenTitleReassignments = 30;
        internal int society_nPeopleForEmpire = 21;
        internal int society_nPeopleForKingdom = 12;
        internal double society_introversionStabilityGain = 1.2;

        internal double temporaryThreatDecay = 0.95;
        internal double threat_takeLocation = 5;
        internal double victory_targetEnshadowmentAvrg = 0.75;

        internal double dark_evilThreatMult = 1.5;
        internal double dark_fleshThreatMult = 2;
        internal double dark_fishmanStartingThreatMult = -0.25;

        public int ability_denounceOtherCooldown = 32;
        public int ability_proposeVoteCooldown = 7;
        public double ability_growFleshThreatAdd = 5;
        internal int ability_shareEvidenceLikingCost = 20;
        internal int ability_switchVoteLikingCost = 20;
        internal double ability_shareEvidencePercentage = 0.75;
        internal int ability_enshadowCost = 4;
        internal int ability_militaryAidDur = 20;
        public int ability_militaryAidAmount = 5;
        public int ability_militaryAidCost = 5;
        public int ability_fishmanRaidCost = 5;
        internal int ability_fishmanRaidMilAdd = 4;
        internal int ability_fishmanRaidTemporaryThreat = 20;
        internal int ability_fishmanLairCost = 8;
        public int ability_fishmanCultOfTheDeep = 7;
        internal int ability_boycottVoteCost = 15;
        internal int ability_cancelVoteCost = 10;
        internal int ability_shortMemories = 5;
        internal double society_pExpandIntoEmpty = 0.1;
        internal double temporaryThreatConversion = 0.01;
        internal int ability_fleshScreamThreatAdd = 10;
        internal int ability_fleshScreamCost = 5;
        internal double threat_temporaryDreadDecay = 0.97;
        public int ability_informationBlackoutCost = 4;
        public int ability_informationBlackoutDuration = 15;
        public int ability_FishmanCultDuration = 15;
        internal double ability_fishmanCultMilRegen = 0.5;
        internal double ability_fishmanCultTempThreat = 1;
        internal double ability_fishmanCultDread = 6;
        public int ability_trustingFoolCost = 5;
        public int ability_trustingFoolCooldown = 25;
        public int ability_fearmongerTempThreat = 25;
        public int ability_fearmongerCooldown = 15;
        internal int ability_apoptosisCost = 15;

        public void loadFromFile()
        {
            //Placeholder
        }
    }
}
