using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class ThreatItem
    {
        public SocialGroup group;
        public Map map;
        public Person p;
        public float zeitgeist = 1;
        public float threat;
        public float accumulator;
        public float investigatorReports;

        public enum form { NONE, DARK_AGENTS,DARK_GODS,PARAHUMANS,DISEASE,DARKNESS}
        public form special = 0;

        public int responseCode = RESPONSE_MILITARY;
        public static int RESPONSE_WEALTH = 0;
        public static int RESPONSE_MILITARY = 1;
        public static int RESPONSE_INFECTION = 2;
        public static int RESPONSE_DARK = 3;
        public static int RESPONSE_AGENTS = 4;
        public static string[] responseNames = new string[] { "Wealth","Military", "Quarantine", "Enlightment", "Inquisition" };

        public static float param_at_war = 5;
        public static float param_strength = 2;
        public static float param_territory = 2;
        public static float param_zeitgeistDecay = 0.95f;
        public static float param_accumulatorDecay = 0.97f;
        public static float param_stance_offensive = 1.75f;
        public static float param_stance_defensive = 1.25f;

        public ThreatItem(Map map,Person parent)
        {
            this.map = map;
            this.p = parent;
        }

        public bool isSame(ThreatItem other)
        {
            if (group != null)
            {
                return group == other.group;
            }
            return special == other.special;
        }
        public void setTo(ThreatItem other)
        {
            group = other.group;
            special = other.special;
        }

        public void cycle()
        {
            //Zeit decays towards 1 exponentially
            zeitgeist -= 1;
            zeitgeist *= param_zeitgeistDecay;
            zeitgeist += 1;

            investigatorReports *= param_accumulatorDecay;
            if (Math.Abs(investigatorReports ) < 0.01) { investigatorReports = 0; }
            accumulator *= param_accumulatorDecay;

            this.setThreat(null);
        }

        public string getTitle()
        {
            if (group == null)
            {
                if (special == form.DARK_AGENTS)
                {
                    return "Dark Agents";
                }
                if (special == form.DARK_GODS)
                {
                    return "Dark Gods";
                }
                if (special == form.PARAHUMANS)
                {
                    return "Parahumans";
                }
                if (special == form.DISEASE)
                {
                    return "Disease";
                }
                if (special == form.DARKNESS)
                {
                    return "Darkness of the Soul";
                }
                return "UNKNOWN";
            }else
            {
                return group.getName();
            }
        }

        public void setThreat(List<string> verbose)
        {
            if (group == null)
            {
                threat = 0;
                if (special == form.DARK_AGENTS)
                {
                    responseCode = RESPONSE_AGENTS;

                    threat = accumulator;
                    if (verbose != null) { verbose.Add("" + (int)(threat) + ": Evidence Found"); }

                    //float mult = 0.5f + (map.panic.worldPanic / 25);
                    //threat *= mult;
                    //if (verbose != null)
                    //{
                    //    int iOut = (int)(mult * 100);
                    //    double dOut = iOut;
                    //    dOut /= 100;
                    //   verbose.Add("X " + dOut + ": World Panic");
                    //}

                    float invMult = 1 + investigatorReports;
                    if (verbose != null && invMult != 1)
                    {
                        int iOut = (int)(invMult * 100);
                        double dOut = iOut;
                        dOut /= 100;
                        verbose.Add("X " + dOut + ": Investigator Reports");
                    }
                    threat *= invMult;
                    

                    //mult = p.awarenessLevel * 0.25f;
                    //threat *= mult;
                    //if (verbose != null)
                    //{
                    //    int iOut = (int)(mult * 100);
                    //    double dOut = iOut;
                    //    dOut /= 100;
                    //    verbose.Add("X " + dOut + ": Person Awareness");
                    //}
                }
                else if (special == form.PARAHUMANS)
                {
                    responseCode = RESPONSE_INFECTION; 


                    if (p == null) { throw new Exception(); }
                    if (p.domain == null) { throw new Exception(); }
                    if (p.domain.population == null) { throw new Exception(); }
                    threat = (float)(1 - p.domain.population.species[map.globalist.species_basic.index]);
                    threat *= 350;
                    if (verbose != null) { verbose.Add("" + (int)(threat) + ": Parahumans in City"); }
                    
                }
                else if (special == form.DISEASE)
                {
                    responseCode = RESPONSE_INFECTION;
                    
                    if (p == null) { throw new Exception(); }
                    if (p.domain == null) { throw new Exception(); }
                    if (p.domain.population == null) { throw new Exception(); }
                    float totalDisease = 0;
                    for (int i = 0; i < p.domain.population.diseases.Length; i++)
                    {
                        totalDisease += (float)p.domain.population.diseases[i];
                    }

                    threat = (totalDisease);
                    threat *= 600;
                    if (verbose != null) { verbose.Add("" + (int)(threat) + ": Disease in City"); }
                    
                }
                else if (special == form.DARKNESS)
                {
                    responseCode = RESPONSE_DARK;

                    int nKnown = 0;
                    foreach (KnowledgeDark know in p.knowledge)
                    {
                        if (know.person == null) { continue; }
                        if (know.know != KnowledgeDark.knowType.IsPartiallyDark) { continue; }
                        if (know.person.domain == null) { continue; }
                        if (know.person.domain.loc.settlement != know.person.domain) { continue; }
                        if (know.person.domain.lordSlot.person != know.person) { continue; }
                        if (know.person.shadow < p.shadow + map.para.evidence_shadowDetectionThreshold) { continue;}

                        nKnown += 1;
                    }

                    threat = nKnown;
                    threat *= 100;
                    if (verbose != null) { verbose.Add("" + (int)(threat) + ": Known Dark-touched Nobles"); }
                }


                {
                    //Zeitgeist
                    {
                        threat *= zeitgeist;

                        int iOut = (int)(zeitgeist * 100);
                        double dOut = iOut;
                        dOut /= 100;
                        if (verbose != null) { verbose.Add("X " + dOut + ": Zeitgeist"); }
                    }
                }
            }
            else
            {
                responseCode = RESPONSE_MILITARY;

                double newThreat = 50;
                if (verbose != null) { verbose.Add("Base: " + newThreat); }

                if (group.threatMult != 1)
                {
                    newThreat *= group.threatMult;

                    int iOut = (int)(group.threatMult * 100);
                    double dOut = iOut;
                    dOut /= 100;
                    if (verbose != null) { verbose.Add("X " + dOut + ": From Other's Type"); }
                }

                float invMult = 1 + investigatorReports;
                if (verbose != null && invMult != 1)
                {
                    int iOut = (int)(invMult * 100);
                    double dOut = iOut;
                    dOut /= 100;
                    verbose.Add("X " + dOut + ": Investigator Reports");
                }
                threat *= invMult;

                if (group.threatGenerated > 0.025)
                {
                    newThreat *= 1 + group.threatGenerated;

                    int iOut = (int)(group.threatGenerated * 100);
                    double dOut = 100 + iOut;
                    dOut /= 100;
                    if (verbose != null) { verbose.Add("X " + dOut + ": From Other's Actions"); }

                }

                //Multiplier from relationship
                if (p.society.getRel(group).state == DipRel.dipState.alliance)
                {
                    if (verbose != null) { verbose.Add("Zero. Have Alliance"); }
                    newThreat = 0;
                }else if (p.society.getRel(group).state == DipRel.dipState.war)
                {
                    if (verbose != null) { verbose.Add("X " + param_at_war + ": At War"); }
                    newThreat *= param_at_war;
                }

                //From their stance
                if (group is Society)
                {
                    Society soc = (Society)group;
                    if (soc.stance == Society.STANCE_OFFENSIVE)
                    {
                        newThreat *= param_stance_offensive;
                        if (verbose != null) { verbose.Add("X " + param_stance_offensive + ": Offensive Stance"); }
                    }
                    //if (soc.stance == Society.STANCE_DEFENSIVE)
                    //{
                    //    newThreat *= param_stance_defensive;
                    //    if (verbose != null) { verbose.Add("X " + param_stance_defensive + ": Defensive Stance"); }
                    //}
                }

                float nLocOurs = 0;
                float nLocTheirs = 0;
                //Multiplier from distance. Pick closest point to person's domain, shrink based on distance
                if (p.slot != null && p.slot.domain != null)
                {
                    double dist = -1;
                    foreach (Location loc in map.locations)
                    {
                        if (loc.soc == group)
                        {
                            nLocTheirs += 1;
                            double local = map.getDist(loc.hex, p.slot.domain.loc.hex);
                            if (dist == -1 || local < dist)
                            {
                                dist = local;
                            }
                        }
                        else if (loc.soc == p.society)
                        {
                            nLocOurs += 1;
                        }
                    }

                    if (dist != -1)
                    {
                        dist /= Math.Max(map.sizeX, map.sizeY);//It's now 0 at 0, just above 1.0 at max distance (due to diagonals)
                        double adaptedDist = Math.Max(0,1 - dist);
                        adaptedDist += 0.1f;
                        adaptedDist = (float)Math.Pow(adaptedDist, 1.75);
                        newThreat *= adaptedDist;

                        int iOut = (int)(adaptedDist * 100);
                        double dOut = iOut;
                        dOut /= 100;
                        if (verbose != null) { verbose.Add("X " + dOut + ": From Distance"); }
                    }
                }

                //Relative Strength
                float ours = p.society.getMilitaryStrength();
                float theirs = group.getMilitaryStrength();
                float strengthThreat = 0;
                if (ours > 0 || theirs > 0)
                {
                    float mult = theirs / (ours + theirs);//1 if they are infinitely more powerful, 0 if we are, 0.5 at equal
                    mult *= param_strength;
                    strengthThreat = mult;
                }
                float territoryMult = 0;
                if (nLocOurs > 0 || nLocTheirs > 0)
                {
                    float mult = nLocTheirs / (nLocOurs + nLocTheirs);//1 if they are infinitely more powerful, 0 if we are, 0.5 at equal
                    mult *= param_territory;
                    territoryMult = mult;

                }

                {
                    float mult = Math.Max(strengthThreat, territoryMult);
                    newThreat *= mult;

                    int iOut = (int)(mult * 100);
                    double dOut = iOut;
                    dOut /= 100;
                    if (verbose != null) { verbose.Add("X " + dOut + ": Military & Territory"); }
                }

                //Zeitgeist
                {
                    newThreat *= zeitgeist;

                    int iOut = (int)(zeitgeist * 100);
                    double dOut = iOut;
                    dOut /= 100;
                    if (verbose != null) { verbose.Add("X " + dOut + ": Zeitgeist"); }
                }

                threat = (float)newThreat;
            }
        }
    }
}
