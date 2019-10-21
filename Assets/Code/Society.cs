﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Society : SocialGroup
    {
        public List<Person> people = new List<Person>();
        public Title sovreign;
        public List<Title> titles = new List<Title>();

        public List<TitleLanded> unclaimedTitles = new List<TitleLanded>();

        public enum militaryPosture { introverted,defensive,offensive};
        public militaryPosture posture = militaryPosture.introverted;
        public SocialGroup defensiveTarget;
        public SocialGroup offensiveTarget;
        public VoteSession voteSession;
        public int voteCooldown = 0;
        internal List<EconEffect> econEffects = new List<EconEffect>();
        public bool isRebellion = false;
        public List<KillOrder> killOrders = new List<KillOrder>();
        public List<Zeit> zeits = new List<Zeit>();
        private Location capital;

        public int instabilityTurns;
        public double data_loyalLordsCap;
        public double data_rebelLordsCap;
        public int turnSovreignAssigned = -1;

        public LogBox logbox;
        public double data_societalStability;

        public Society(Map map) : base(map)
        {
            setName("DEFAULT_SOC_NAME");
            sovreign = new Title_Sovreign(this);
            titles.Add(sovreign);
        }

        public override void turnTick()
        {
            base.turnTick();
            debug();
            processExpirables();
            processKillOrders();
            processStability();
            checkTitles();
            processWarExpansion();
            processTerritoryExchanges();
            processVoting();
            checkPopulation();//Add people last, so new people don't suddenly arrive and act before the player can see them
            checkAssertions();
            log();
        }

        public void checkAssertions()
        {
            if (titles.Count == 0) { throw new Exception("Sovreign title not present"); }
        }

        public void log()
        {
            if (World.logging)
            {
                if (logbox == null)
                {
                    logbox = new LogBox(this);
                }
                logbox.takeLine("--------Turn " + map.turn + "------");
            }
        }

        public List<Person> getEnthrallables()
        {
            List<Person> opts = new List<Person>();
            foreach (Person p in people)
            {
                if (p.title_land != null)
                {
                    opts.Add(p);
                }
            }
            int nToChoose = opts.Count / 4;
            if (nToChoose < 1) { nToChoose = 1; }
            

            opts.Sort(new Sorter_PersonByPrestige());

            List<Person> reply = new List<Person>();
            for (int i = 0; i < nToChoose; i++)
            {
                reply.Add(opts[i]);
            }
            return reply;
        }

        public void processKillOrders()
        {
            foreach (KillOrder order in killOrders)
            {
                if (people.Contains(order.person))
                {
                    order.person.die("Executed by " + this.getName() + ". Reason: " + order.reason);
                }
            }
        }
        public void processStability()
        {
            data_loyalLordsCap = 0;
            data_rebelLordsCap = 0;
            if (getSovreign() == null)
            {
                instabilityTurns = 0;
                return;
            }

            List<Person> rebels = new List<Person>();
            foreach (Person p in people)
            {
                if (p.title_land == null) { continue; }
                if (p.getRelation(getSovreign()).getLiking() <= map.param.society_rebelLikingThreshold)
                {
                    data_rebelLordsCap += p.title_land.settlement.getMilitaryCap();
                    rebels.Add(p);
                }
                else
                {
                    data_loyalLordsCap += p.title_land.settlement.getMilitaryCap();
                }
            }

            if (data_loyalLordsCap + data_rebelLordsCap <= 0)
            {
                data_societalStability = 1;
            }
            else
            {
                data_societalStability = (data_loyalLordsCap - data_rebelLordsCap) / (data_loyalLordsCap + data_rebelLordsCap);
            }

            double introBonus = 1;
            if (posture == militaryPosture.introverted)
            {
                introBonus = 1.2;
            }
            if (data_rebelLordsCap >= data_loyalLordsCap*introBonus)
            {
                instabilityTurns += 1;
                if (instabilityTurns >= map.param.society_instablityTillRebellion)
                {
                    triggerCivilWar(rebels);
                }
            }
            else
            {
                instabilityTurns = 0;
            }
        }

        public void triggerCivilWar(List<Person> rebelsTotal)
        {
            World.log(this.getName() + " falls into civil war as " + rebelsTotal.Count + " out of " + people.Count + " nobles declare rebellion against " + getSovreign().getFullName());

            List<Province> seenProvinces = new List<Province>();
            List<List<Person>> rebelsByProvince = new List<List<Person>>();
            List<Person> unmappedRebels = new List<Person>();
            foreach (Person p in rebelsTotal)
            {
                if (p.title_land == null) { unmappedRebels.Add(p); continue; }
                if (seenProvinces.Contains(p.title_land.settlement.location.province))
                {
                    int ind = seenProvinces.IndexOf(p.title_land.settlement.location.province);
                    rebelsByProvince[ind].Add(p);
                }
                else
                {
                    seenProvinces.Add(p.title_land.settlement.location.province);
                    rebelsByProvince.Add(new List<Person>());
                    rebelsByProvince[rebelsByProvince.Count - 1].Add(p);
                }
            }

            if (rebelsByProvince.Count == 0)
            {
                World.log("No rebels had any territory. Rebellion called off");
                return;
            }

            rebelsByProvince[0].AddRange(unmappedRebels);

            World.log("Rebellion has " + seenProvinces.Count + " provinces");

            for (int k = 0; k < seenProvinces.Count; k++)
            {
                List<Person> rebels = rebelsByProvince[k];
                Society rebellion = new Society(map);
                map.socialGroups.Add(rebellion);
                rebellion.setName(seenProvinces[k].name + " rebellion");
                rebellion.isRebellion = true;
                if (Eleven.random.Next(2) == 0)
                {
                    rebellion.posture = militaryPosture.defensive;
                }
                foreach (Person p in rebels)
                {
                    if (p.title_land != null)
                    {
                        p.title_land.settlement.location.soc = rebellion;
                    }
                    this.people.Remove(p);
                    rebellion.people.Add(p);
                    p.society = rebellion;
                }

                double proportionalStrength = 0;
                rebellion.computeMilitaryCap();
                this.computeMilitaryCap();

                if (this.maxMilitary > 0 || rebellion.maxMilitary > 0)
                {
                    proportionalStrength = this.maxMilitary / (this.maxMilitary + rebellion.maxMilitary);
                    rebellion.currentMilitary = this.currentMilitary * (1 - proportionalStrength);
                    this.currentMilitary = this.currentMilitary * proportionalStrength;
                }

                if (getSovreign() != null)
                {
                    KillOrder killSovreign = new KillOrder(getSovreign(), "Rebellion against tyranny");
                    rebellion.killOrders.Add(killSovreign);
                }

                foreach (Person p in rebels)
                {
                    KillOrder killRebel = new KillOrder(p, "Rebelled against sovreign");
                    this.killOrders.Add(killRebel);

                }
                this.map.declareWar(rebellion, this);
            }
        }

        public void processExpirables()
        {
            if (offensiveTarget != null && offensiveTarget.checkIsGone()) { offensiveTarget = null; }

            List<EconEffect> rems = new List<EconEffect>();
            foreach (EconEffect effect in econEffects)
            {
                bool hasFrom = false;
                bool hasTo = false;
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this && loc.settlement != null && loc.settlement.econTraits().Contains(effect.from)){ hasFrom = true; break; }
                }
                if (hasFrom == false) { effect.durationLeft = 0; }//No longer valid
                else
                {
                    foreach (Location loc in map.locations)
                    {
                        if (loc.soc == this && loc.settlement != null && loc.settlement.econTraits().Contains(effect.to)) { hasTo = true; break; }
                    }
                    if (hasTo == false) { effect.durationLeft = 0; }//No longer valid
                }
                if (effect.durationLeft > 0)
                {
                    effect.durationLeft -= 1;
                    }
                if (effect.durationLeft == 0) { rems.Add(effect); }
            }
            foreach (EconEffect effect in rems)
            {
                econEffects.Remove(effect);
            }

            if (isRebellion)
            {
                if (isAtWar())
                {
                    if (this.getCapital() != null)
                    {
                        isRebellion = false;
                        World.log(this.getName() + " has successfully defended itself and broken away properly. Renaming now");
                        this.setName(this.getCapital().shortName);
                    }
                }
            }
        }
        public override bool isProtagonist()
        {
            foreach (Person p in people)
            {
                if (p.state == Person.personState.enthralled) { return true; }
            }
            return false;
        }
        public override string getName()
        {
            string basic = base.getName();
            if (this.people.Count > map.param.society_nPeopleForEmpire)
            {
                return "Empire of " + basic;
            }
            else if (this.people.Count > map.param.society_nPeopleForKingdom)
            {
                if (this.getSovreign() == null || this.getSovreign().isMale)
                {
                    return "Kingdom of " + basic;
                }
                else
                {
                    return "Queendom of " + basic;
                }
            }
            else
            {
                return "Duchy of " + basic;
            }
        }

        public Location getCapital()
        {
            if (capital == null || capital.soc != this)
            {
                computeCapital();
                return capital;
            }
            if (capital.settlement.location.soc != this) { computeCapital(); }
            return capital;
        }

        public void computeCapital()
        {
            double bestPrestige = -100000;
            foreach (Location loc in map.locations)
            {
                if (loc.soc != this) { continue; }
                if (loc.settlement == null) { continue; }
                if (loc.settlement.basePrestige > bestPrestige)
                {
                    bestPrestige = loc.settlement.basePrestige;
                    capital = loc;
                }
            }
        }

        public override double getThreat(List<ReasonMsg> msgs)
        {
            double threat = base.getThreat(msgs);
            if (this.posture == militaryPosture.offensive)
            {
                int percent = (int)(100 * map.param.society_threatMultFromOffensivePosture);
                if (msgs != null)
                {
                    msgs.Add(new ReasonMsg("Offensive Posture (+" + percent + "%)", threat));
                }
                threat *= 1 + (map.param.society_threatMultFromOffensivePosture);
            }
            return threat;
        }


        public void processTerritoryExchanges()
        {
            bool hasWar = false;
            foreach (DipRel rel in getAllRelations())
            {
                if (rel.state == DipRel.dipState.war)
                {
                    hasWar = true;
                    break;
                }
            }
            if (!hasWar)
            {
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this)
                    {
                        if (loc.isForSocieties == false || loc.hex.getHabilitability() < map.param.mapGen_minHabitabilityForHumans)
                        {
                            if (loc.settlement != null)
                            {
                                loc.settlement = null;
                            }
                            loc.soc = null;
                        }

                        if (loc.isForSocieties && loc.settlement == null && loc.isForSocieties && loc.hex.getHabilitability() >= map.param.mapGen_minHabitabilityForHumans)
                        {
                            if (loc.isMajor)
                            {
                                loc.settlement = new Set_City(loc);
                            }
                            else
                            {
                                loc.settlement = new Set_Fort(loc);
                            }
                        }
                    }
                }
            }
        }
        //Check to see if you need to build outposts to reach an enemy you're at war with
        public void processWarExpansion()
        {
            List<SocialGroup> neighbours = this.getNeighbours();
            foreach (DipRel rel in getAllRelations())
            {
                if (rel.state == DipRel.dipState.war && rel.war.att == this)
                {
                    if (neighbours.Contains(rel.other(this)) == false)
                    {
                        //We need to build an outpost towards them
                        Location[] pathTo = map.getEmptyPathTo(this, rel.other(this));
                        if (pathTo != null && pathTo.Length > 1 && pathTo[1].soc == null)
                        {
                            pathTo[1].soc = this;
                        }
                    }
                }
            }
        }

        public void debug()
        {
            /*
            if (this.getSize() < 5)
            {
                int nWars = 0;
                foreach (SocialGroup other in getNeighbours())
                {
                    if (getRel(other).state == DipRel.dipState.war)
                    {
                        nWars += 1;
                    }

                }
                if (nWars == 0)
                {
                    int c = 0;
                    SocialGroup choice = null;
                    foreach (SocialGroup other in getNeighbours())
                    {
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            choice = other;
                        }
                    }
                    if (choice != null)
                    {
                        map.declareWar(this, choice);
                    }
                }
            }
            */
        }

        public void processVoting()
        {
            if (voteSession == null)
            {
                if (voteCooldown > 0) { voteCooldown -= 1; return; }

                Person proposer = null;
                foreach (Person p in people)
                {
                    if (p.state == Person.personState.enthralled) { continue; }
                    if (proposer == null)
                    {
                        proposer = p;
                    }
                    else
                    {
                        int myDelta = map.turn - p.lastVoteProposalTurn;
                        int theirDelta = map.turn - proposer.lastVoteProposalTurn;
                        if (myDelta > theirDelta)
                        {
                            proposer = p;
                        }
                    }
                }
                if (proposer != null)
                {
                    proposer.lastVoteProposalTurn = map.turn;
                    VoteIssue issue = proposer.proposeVotingIssue();
                    if (issue == null) {return; }

                    //Otherwise, on with voting for this new thing
                    voteSession = new VoteSession();
                    voteSession.issue = issue;
                    voteSession.timeRemaining = map.param.society_votingDuration;
                    if (World.logging && logbox != null) { logbox.takeLine("Starting voting on " + voteSession.issue.ToString()); }
                }
            }
            else
            {
                if (voteSession.issue.stillValid(map) == false)
                {
                    voteSession = null;
                    World.log("Vote session no longer valid");
                    return;
                }
                if (voteSession.timeRemaining > 0) { voteSession.timeRemaining -= 1; return; }
                
                foreach (Person p in people)
                {
                    if (World.logging) { p.log.takeLine("Voting on " + voteSession.issue); }
                    double highestWeight = 0;
                    VoteOption bestChoice = null;
                    foreach (VoteOption option in voteSession.issue.options)
                    {
                        List<ReasonMsg> msgs = new List<ReasonMsg>();
                        double u = voteSession.issue.computeUtility(p, option, msgs);
                        option.msgs.Add(p, msgs);
                        if (u > highestWeight || bestChoice == null)
                        {
                            bestChoice = option;
                            highestWeight = u;
                        }
                        if (World.logging) {
                            p.log.takeLine(" " + option.fixedLenInfo() + "  " + u);
                            foreach (ReasonMsg msg in msgs)
                            {
                                p.log.takeLine("     " + Eleven.toFixedLen(msg.value, 5) +  msg.msg);
                            }
                        }
                    }
                    bestChoice.votesFor.Add(p);
                    bestChoice.votingWeight += p.prestige;

                }

                double topVote = 0;
                VoteOption winner = null;
                foreach (VoteOption option in voteSession.issue.options)
                {
                    if (option.votingWeight > topVote || winner == null)
                    {
                        winner = option;
                        topVote = option.votingWeight;
                    }

                    voteSession.issue.changeLikingForVotes(option);
                }

                if (World.logging && logbox != null) { logbox.takeLine("End voting on " + voteSession.issue.ToString()); }
                if (World.logging && logbox != null) { logbox.takeLine("    Winning option: " + winner.info()); }
                voteSession.issue.implement(winner);
                voteSession = null;
            }
        }

        public void checkTitles()
        {
            unclaimedTitles.Clear();
            foreach (Person p in people)
            {
                if (p.title_land != null)
                {
                    //Settlement is razed
                    if (p.title_land.settlement.location.settlement != p.title_land.settlement)
                    {
                        log(p.getFullName() + " has lost title " + p.title_land.getName() + " has it no longer exists.");
                        p.title_land = null;
                    }
                    if (p.title_land.settlement.location.soc != this)
                    {
                        log(p.getFullName() + " has lost title " + p.title_land.getName() + " has it is no longer owned by their society, " + this.getName());
                        p.title_land = null;
                    }
                }
            }
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    if (loc.settlement != null && loc.settlement.title != null && loc.settlement.title.heldBy == null)
                    {
                        unclaimedTitles.Add(loc.settlement.title);
                    }
                }
            }
            

            foreach (Title t in titles)
            {
                t.turnTick();
            }
        }
        
        public void checkPopulation()
        {
            foreach (Person p in people)
            {
                p.turnTick();
            }

            //Insta-add enough to make up the numbers
            while (people.Count < map.param.soc_untitledPeople + unclaimedTitles.Count)
            {
                Person p = new Person(this);
                log(p.getFullName() + " has risen to note in the society of " + this.getName());
                people.Add(p);
            }

            //Remove one per turn if you're over cap
            int untitledPeople = 0;
            foreach (Person p in people)
            {
                if (p.title_land == null) { untitledPeople += 1; }
                untitledPeople -= unclaimedTitles.Count;//These are presumably about to be handed out
            }
            if (untitledPeople > map.param.soc_maxUntitledPeople)
            {
                Person lastUntitled = null;
                foreach (Person p in people)
                {
                    if (p.title_land == null)
                    {
                        lastUntitled = p;
                    }
                }
                if (lastUntitled != null)
                {
                    log(lastUntitled.getFullName() + " has no title, and has lost lordship in " + this.getName());
                    people.Remove(lastUntitled);
                }
            }
        }

        internal bool isAtWar()
        {
            foreach (DipRel rel in relations.Values)
            {
                if (rel.other(this).isGone()) { continue; }
                if (rel.state == DipRel.dipState.war) { return true; }
            }
            return false;
        }

        public Person getSovreign()
        {
            return sovreign.heldBy;
        }
        public void log(String msg)
        {
            World.log(msg);
        }
    }
}
