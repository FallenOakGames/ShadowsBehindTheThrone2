using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Person
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
                targetPrestige += title_land.settlement.basePrestige;
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
                    item.reasons.Add(new ReasonMsg("Group Threat", value));
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
                    int intInfoAvailability = (int)(infoAvailability);
                    item.reasons.Add(new ReasonMsg("Information (% kept)", intInfoAvailability));
                    value *= infoAvailability;

                    value /= (society.currentMilitary + (society.maxMilitary/2)) + 1;

                    value *= map.param.person_threatMult;

                    item.threat = value;
                }
            }
        }

        public double getRelBaseline(Person other)
        {
            if (other == this) { return 100; }
            return map.param.relObj_defaultLiking;
        }

        public Map map { get { return society.map; } }

        public string getFullName()
        {
            return getTitles() + " " + firstName;
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

        public void die(string v)
        {
            World.log(v);
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

        public void logVote(VoteIssue issue)
        {
            if (World.logging)
            {
                string line = "  " + issue.ToString() + " for soc " + issue.society.getName();
                log.takeLine(line);
                foreach (VoteOption opt in issue.options)
                {
                    line = "     " + opt.fixedLenInfo();
                    line += " U " + Eleven.toFixedLen(issue.computeUtility(this, opt, new List<ReasonMsg>()),12);
                    log.takeLine(line);
                }
            }
        }

        public VoteIssue proposeVotingIssue()
        {
            //Note we start at 1 utility. This means no guaranteed negative/near-zero utility options will be proposed
            double bestU = 1;
            VoteIssue bestIssue = null;

            bool existFreeTitles = false;

            if (World.logging) { log.takeLine("Proposing vote on turn " + map.turn); }


            foreach (Location loc in map.locations)
            {
                if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && loc.settlement.title.heldBy == null)
                {
                    existFreeTitles = true;
                }
            }

            VoteIssue issue;

            //Unlanded titles can be distributed
            //Assignment of sovreign takes priority over any other voting, in the minds of the lords and ladies
            foreach (Title t in society.titles)
            {
                if (t.heldBy != null && (map.turn - t.turnLastAssigned < map.param.society_minTimeBetweenTitleReassignments)) { continue; }
                issue = new VoteIssue_AssignTitle(society, this, t);

                //Everyone is eligible
                foreach (Person p in society.people)
                {
                    VoteOption opt = new VoteOption();
                    opt.person = p;
                    issue.options.Add(opt);
                }
                foreach (VoteOption opt in issue.options)
                {
                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU || (t == society.sovreign && t.heldBy == null))//Note we force them to vote on a sovereign if there is none
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                }
                logVote(issue);
            }

            if (society.getSovreign() != null)
            {
                foreach (Location loc in map.locations)
                {
                    //If there are unhanded out titles, only consider those. Else, check all.
                    //Maybe they could be rearranged (handed out or simply swapped) in a way which could benefit you
                    //if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && ((!existFreeTitles) || (loc.settlement.title.heldBy == null)))

                    //We're now stopping them suggesting this on places with existing nobles, as that lead to undue amounts of swapping
                    if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && (loc.settlement.title.heldBy == null))
                    {
                        //if (map.turn - loc.turnLastAssigned  < Params.society_minTimeBetweenLocReassignments) { continue; }
                        issue = new VoteIssue_AssignLandedTitle(society, this, loc.settlement.title);
                        if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                                                                                                                   //Everyone is eligible
                        foreach (Person p in society.people)
                        {
                            if (p.title_land != null) { continue; }//Again, to prevent constant shuffling
                            VoteOption opt = new VoteOption();
                            opt.person = p;
                            issue.options.Add(opt);
                        }

                        foreach (VoteOption opt in issue.options)
                        {
                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                            }
                        }
                        logVote(issue);
                    }
                }

                //Check to see if you want to economically rebalance the economy
                if (this.title_land != null)
                {
                    HashSet<EconTrait> mine = new HashSet<EconTrait>();
                    HashSet<EconTrait> all = new HashSet<EconTrait>();


                    foreach (EconTrait trait in title_land.settlement.econTraits())
                    {
                        mine.Add(trait);
                    }
                    foreach (Location loc in map.locations)
                    {
                        if (loc.soc == society && loc.settlement != null)
                        {
                            foreach (EconTrait trait in loc.settlement.econTraits())
                            {
                                all.Add(trait);
                            }
                        }
                    }

                    foreach (EconTrait econ_from in all)
                    {
                        if (mine.Contains(econ_from)) { continue; }//Don't take from yourself
                        foreach (EconTrait econ_to in mine)
                        {
                            issue = new VoteIssue_EconomicRebalancing(society, this);
                            //Allow them to spam econ votes
                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                            bool present = false;
                            foreach (EconEffect effect in society.econEffects)
                            {
                                if (effect.from == econ_from && effect.to == econ_to) { present = true; }
                                if (effect.to == econ_from && effect.from == econ_to) { present = true; }
                            }
                            if (present) { continue; }

                            //We have our two options (one way or the other)
                            VoteOption opt1 = new VoteOption();
                            opt1.econ_from = econ_from;
                            opt1.econ_to = econ_to;
                            issue.options.Add(opt1);
                            VoteOption opt2 = new VoteOption();
                            opt2.econ_from = econ_to;
                            opt2.econ_to = econ_from;
                            issue.options.Add(opt2);

                            foreach (VoteOption opt in issue.options)
                            {
                                //Random factor to prevent them all rushing a singular voting choice
                                double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                                if (localU > bestU)
                                {
                                    bestU = localU;
                                    bestIssue = issue;
                                }
                            }

                            logVote(issue);
                        }
                    }
                }

                //Check to see if you want to alter offensive military targetting
                issue = new VoteIssue_SetOffensiveTarget(society, this);
                foreach (SocialGroup neighbour in map.getExtendedNeighbours(society))
                {
                    VoteOption option = new VoteOption();
                    option.group = neighbour;
                    issue.options.Add(option);
                }
                foreach (VoteOption opt in issue.options)
                {
                    //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU)
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                }
                logVote(issue);

                //Check to see if you want to alter defensive military targetting
                issue = new VoteIssue_SetDefensiveTarget(society, this);
                foreach (ThreatItem item in threatEvaluations)
                {
                    if (item.group == null) { continue; }
                    VoteOption option = new VoteOption();
                    option.group = item.group;
                    issue.options.Add(option);
                }
                foreach (VoteOption opt in issue.options)
                {
                    //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU)
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                }
                logVote(issue);

                //Change military posture, to either improve defence, fix internal problems, or attack an enemy
                issue = new VoteIssue_MilitaryStance(society, this);
                for (int i = 0; i < 3; i++)
                {
                    VoteOption opt = new VoteOption();
                    opt.index = i;
                    issue.options.Add(opt);
                }
                foreach (VoteOption opt in issue.options)
                {
                    //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, opt, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU)
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                }
                logVote(issue);

                //Check to see if you want to declare war
                //You need to be in offensive posture to be allowed to do so
                if (society.offensiveTarget != null && society.posture == Society.militaryPosture.offensive && society.getRel(society.offensiveTarget).state != DipRel.dipState.war)
                {
                    issue = new VoteIssue_DeclareWar(society, society.offensiveTarget, this);
                    VoteOption option_0 = new VoteOption();
                    option_0.index = 0;
                    issue.options.Add(option_0);

                    VoteOption option_1 = new VoteOption();
                    option_1.index = 1;
                    issue.options.Add(option_1);
                    
                    //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU)
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                    logVote(issue);
                }

                //Check to see if you want to defensively vassalise yourself
                //You need to be in defensive posture to be allowed to do so
                if (society.offensiveTarget != null && society.posture == Society.militaryPosture.defensive && (society.isAtWar() == false))
                {
                    foreach (SocialGroup sg in society.getNeighbours())
                    {
                        if (sg is Society == false) { continue; }
                        if (sg == this.society) { continue; }
                        Society other = (Society)sg;
                        if (other.defensiveTarget == this.society.defensiveTarget)
                        {
                            issue = new VoteIssue_Vassalise(society, other, this);
                            VoteOption option_0 = new VoteOption();
                            option_0.index = 0;
                            issue.options.Add(option_0);

                            VoteOption option_1 = new VoteOption();
                            option_1.index = 1;
                            issue.options.Add(option_1);
                            
                            //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                            //Random factor to prevent them all rushing a singular voting choice
                            double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                            }
                            logVote(issue);
                        }
                    }
                }
                
                //Check if you want to execute someone
                if (society.posture == Society.militaryPosture.introverted)
                {
                    foreach (Person p in society.people)
                    {
                        if (p == this) { continue; }
                        issue = new VoteIssue_JudgeSuspect(society, p, this);
                        VoteOption option_0 = new VoteOption();
                        option_0.index = 0;
                        issue.options.Add(option_0);

                        VoteOption option_1 = new VoteOption();
                        option_1.index = 1;
                        issue.options.Add(option_1);
                        
                        //if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                        //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, option_1, new List<ReasonMsg>()) * Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                        }
                        logVote(issue);
                    }
                }
                
            }

            if (bestIssue != null)
            {
                if (World.logging)
                {
                    log.takeLine("CHOSE: " + bestIssue.ToString());
                }
            }
            lastProposedIssue = bestIssue;
            return bestIssue;
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
