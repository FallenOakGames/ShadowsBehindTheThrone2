using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public partial class Person
    {
        public string firstName;
        public bool isMale = Eleven.random.Next(2) == 0;
        public List<Title> titles = new List<Title>();
        public TitleLanded title_land;
        public Society society;
        public Dictionary<Person, RelObj> relations = new Dictionary<Person, RelObj>();
        public double prestige = 1;
        public int lastVoteProposalTurn;
        public VoteIssue lastProposedIssue;
        public GraphicalSlot outer;
        public List<ThreatItem> threatEvaluations = new List<ThreatItem>();
        public LogBox log;
        public double evidence;
        public double shadow;
        public int imgIndBack = -1;
        public int imgIndFore = -1;

        public ThreatItem threat_enshadowedNobles;

        public double politics_militarism = Eleven.random.NextDouble() * 2 - 1;

        public enum personState { normal,enthralled,broken};
        public personState state = personState.normal;
        public bool isDead;
        internal Society rebellingFrom;

        public Person(Society soc)
        {
            this.society = soc;
            firstName = TextStore.getName(isMale);

            if (World.logging)
            {
                log = new LogBox(this);
            }

            //Add permanent threats
            threat_enshadowedNobles = new ThreatItem(map,this);
            threat_enshadowedNobles.form = ThreatItem.formTypes.ENSHADOWED_NOBLES;
            threatEvaluations.Add(threat_enshadowedNobles);
        }

        public void turnTick()
        {
            if (World.logging) { log.takeLine("---------Turn " + map.turn + "------------"); }

            processEnshadowment();

            double targetPrestige = map.param.person_defaultPrestige;
            if (title_land != null)
            {
                targetPrestige += title_land.settlement.getPrestige();
            }
            foreach (Title t in titles) { targetPrestige += t.getPrestige(); }
            if (Math.Abs(prestige-targetPrestige) < map.param.person_prestigeDeltaPerTurn)
            {
                prestige = targetPrestige;
            }
            else if (prestige < targetPrestige) { prestige += map.param.person_prestigeDeltaPerTurn; }
            else if (prestige > targetPrestige) { prestige -= map.param.person_prestigeDeltaPerTurn; }

            foreach (RelObj rel in relations.Values)
            {
                rel.turnTick();
            }

            List<Title> rems = new List<Title>();
            foreach (Title t in titles)
            {
                if (t.heldBy != this || t.society != this.society)
                {
                    rems.Add(t);
                }
            }
            foreach (Title t in rems) { titles.Remove(t); }

            computeSuspicionGain();
            processThreats();
        }

        private void computeSuspicionGain()
        {
            foreach (SocialGroup sg in map.socialGroups)
            {
                if (sg is Society == false) { continue; }
                Society soc = (Society)sg;
                foreach (Person p in soc.people)
                {
                    if (p == this) { continue; }
                    double infoAvail = map.getInformationAvailability(this.getLocation(), sg);
                    RelObj rel = getRelation(p);
                    rel.suspicion += infoAvail * p.evidence * map.param.person_suspicionPerEvidence;
                }
            }
        }

        public Location getLocation()
        {
            if (this.title_land != null)
            {
                return this.title_land.settlement.location;
            }
            return this.society.getCapital();
        }

        private void processEnshadowment()
        {
            evidence += shadow * map.param.person_evidencePerShadow;
            if (evidence > 1)
            {
                evidence = 1;
            }
            if (state != personState.broken && state != personState.enthralled) {
                shadow -= map.param.person_shadowDecayPerTurn;
                if (shadow < 0) { shadow = 0; }
            }
            foreach (Person p in society.people)
            {
                if (p == this) { continue; }
                if (p.shadow == 0) { continue; }//Can't inherit if they don't have any, skip to save CPU
                if (p.shadow <= shadow) { continue; }

                double basePrestige = 100;
                if (society.getSovreign() != null) { basePrestige = society.getSovreign().prestige; }
                if (basePrestige < 10) { basePrestige = 10; }
                double multFromPrestige = p.prestige / basePrestige;
                if (multFromPrestige < 0) { multFromPrestige = 0; }
                if (multFromPrestige > 1) { multFromPrestige = 1; }

                double likingMult = Math.Max(0, this.getRelation(p).getLiking())/100;

                double shadowDelta = p.shadow * likingMult * map.param.person_shadowContagionMult * multFromPrestige;//You get enshadowed by people you like/trust
                this.shadow = Math.Min(p.shadow, shadow + shadowDelta);//Don't exceed your donor's shadow
                if (this.shadow > 1) { this.shadow = 1; }
            }

            if (state == personState.normal && shadow == 1)
            {
                this.state = personState.broken;
                map.turnMessages.Add(new MsgEvent(this.getFullName() + " has been fully enshadowed, their soul can no longer resist the dark", MsgEvent.LEVEL_GREEN,true));
            }
        }

        public void processThreats()
        {
            //First up, see if anything needs to be added/removed
            List<ThreatItem> rems = new List<ThreatItem>();
            HashSet<SocialGroup> groups = new HashSet<SocialGroup>();
            foreach (ThreatItem item in threatEvaluations)
            {
                if (item.group != null){
                    if (item.group.isGone())
                    {
                        rems.Add(item);
                    }
                    else if (item.group == society)
                    {
                        rems.Add(item);
                    }
                    else
                    {
                        groups.Add(item.group);
                    }
                }
                item.turnTick();
            }
            foreach (ThreatItem item in rems) { threatEvaluations.Remove(item); }

            foreach (SocialGroup sg in map.socialGroups)
            {
                if (groups.Contains(sg) == false && (sg != society))
                {
                    ThreatItem item = new ThreatItem(map, this);
                    item.group = sg;
                    threatEvaluations.Add(item);
                }
            }

            //Actually do the evaluations here
            foreach (ThreatItem item in threatEvaluations)
            {
                item.threat = 0;
                item.reasons.Clear();
                if (item.group == null)
                {
                    if (item.form == ThreatItem.formTypes.ENSHADOWED_NOBLES)
                    {
                        if (this.state == personState.broken) { continue; }//Broken minded can't fear the darkness
                        foreach (Person p in this.society.people)
                        {
                            RelObj rel = this.getRelation(p);
                            double sus = rel.suspicion*map.param.person_threatFromSuspicion;
                            item.threat += sus;
                            if (sus > 1)
                            {
                                item.reasons.Add(new ReasonMsg("Supicion of " + p.getFullName(), sus));
                            }
                        }
                    }
                }
                else
                {
                    double value = item.group.getThreat(null);
                    item.reasons.Add(new ReasonMsg("Social Group's total threat: ", value));
                    Location sourceLoc = null;
                    //Fear things which are nearby
                    if (this.title_land != null)
                    {
                        sourceLoc = title_land.settlement.location;
                    }
                    //If you don't have a landed title you live in the capital
                    if (sourceLoc == null)
                    {
                        sourceLoc = society.getCapital();
                    }
                    //Fallback to just use the first location, to avoid null exceptions in extreme edge cases
                    if (sourceLoc == null)
                    {
                        sourceLoc = map.locations[0];
                    }

                    double infoAvailability = map.getInformationAvailability(sourceLoc, item.group);
                    int intInfoAvailability = (int)(infoAvailability*100);
                    item.reasons.Add(new ReasonMsg("Information (% kept)", intInfoAvailability));
                    value *= infoAvailability;

                    double militaryStrengthMult = 100 / ((society.currentMilitary + (society.maxMilitary / 2)) + 1);
                    //if (militaryStrengthMult < 50) { militaryStrengthMult = 50; }
                    item.reasons.Add(new ReasonMsg("Military Strength Comparison (% multiplier)", (int)(100*militaryStrengthMult)));
                    value *= militaryStrengthMult; 

                    item.threat = value;
                }
            }
        }

        public ThreatItem getGreatestThreat()
        {
            ThreatItem g = null;
            double s = 0.0;

            foreach (ThreatItem t in threatEvaluations)
            {
                if (t.threat > s)
                {
                    g = t;
                    s = t.threat;
                }
            }

            return g;
        }

        public double getRelBaseline(Person other)
        {
            if (other == this) { return 100; }
            return map.param.relObj_defaultLiking;
        }

        public Map map { get { return society.map; } }

        public string getFullName()
        {
            return ((state == personState.enthralled || state == personState.broken) ? "Dark ":"") +  getTitles() + " " + firstName;
        }

        public RelObj getRelation(Person other)
        {
            if (other == null) { throw new NullReferenceException(); }
            if (relations.ContainsKey(other))
            {
                return relations[other];
            }
            RelObj rel = new RelObj(this, other);
            relations.Add(other, rel);
            return rel;
        }

        public string getTitles()
        {
            double bestPrestige = 0;
            Title bestTitle = null;
            foreach (Title t in titles)
            {
                if (t.getPrestige() > bestPrestige)
                {
                    bestPrestige = t.getPrestige();
                    bestTitle = t;
                }
            }
            if (isMale)
            {
                if (bestTitle != null) { return bestTitle.nameM; }
                if (title_land != null) { return title_land.titleM; }
                return "Lord";
            }
            else
            {
                if (bestTitle != null) { return bestTitle.nameF; }
                if (title_land != null) { return title_land.titleF; }
                return "Lady";
            }
        }

        public string getMilitarismInfo()
        {
            int m = (int)(100*politics_militarism) + 100;
            if (m <= 40)
                return "Very Pacifist";
            else if (m <= 80)
                return "Somewhat Pacifist";
            else if (m <= 120)
                return "No Tendency";
            else if (m <= 160)
                return "Somewhat Walike";
            else
                return "Very Warlike";
        }

        public void die(string v)
        {
            World.log(v);
            double priority = 0;
            bool benefit = true;
            if (state == Person.personState.enthralled)
            {
                benefit = false;
                priority = MsgEvent.LEVEL_RED;
            }
            else if (state == Person.personState.broken)
            {
                benefit = false;
                priority = MsgEvent.LEVEL_ORANGE;
            }
            else
            {
                benefit = true;
                priority = MsgEvent.LEVEL_DARK_GREEN;
            }
            map.turnMessages.Add(new MsgEvent(this.getFullName() + " dies! " + v, priority, benefit));

            society.people.Remove(this);
            if (this.title_land != null)
            {
                this.title_land.heldBy = null;
            }
            foreach (Title t in titles)
            {
                t.heldBy = null;
            }
            isDead = true;
            if (this == map.overmind.enthralled) { map.overmind.enthralled = null; }
        }

        public Sprite getImageBack()
        {
            if (imgIndBack == -1)
            {
                imgIndBack = Eleven.random.Next(map.world.textureStore.layerBack.Count);
            }
            return map.world.textureStore.layerBack[imgIndBack];
        }

        public Sprite getImageMid()
        {
            if (state == personState.normal)
            {
                return map.world.textureStore.person_basic;
            }
            if (state == personState.enthralled)
            {
                return map.world.textureStore.person_dark;
            }
            if (state == personState.broken)
            {
                return map.world.textureStore.person_halfDark;
            }

            return map.world.textureStore.person_basic;
        }
        public Sprite getImageFore()
        {
            if (imgIndFore == -1)
            {
                imgIndFore = Eleven.random.Next(map.world.textureStore.layerFore.Count);
            }
            return map.world.textureStore.layerFore[imgIndFore];
        }
    }
}
