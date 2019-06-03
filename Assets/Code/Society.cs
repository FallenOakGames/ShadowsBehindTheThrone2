using System;
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

        public int instabilityTurns;
        public double data_loyalLordsCap;
        public double data_rebelLordsCap;

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
            processStability();
            checkTitles();
            processVoting();
            checkPopulation();//Add people last, so new people don't suddenly arrive and act before the player can see them
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
                    data_rebelLordsCap += p.title_land.settlement.militaryCapAdd;
                    rebels.Add(p);
                }
                else
                {
                    data_loyalLordsCap += p.title_land.settlement.militaryCapAdd;
                }
            }

            if (data_rebelLordsCap > data_loyalLordsCap)
            {
                instabilityTurns += 1;
                if (instabilityTurns >= map.param.society_instablityTillRebellion)
                {
                    triggerCivilWar(rebels);
                }
            }
        }

        public void triggerCivilWar(List<Person> rebels)
        {
            World.log(this.getName() + " falls into civil war as " + rebels.Count + " out of " + people.Count + " nobles declare rebellion against " + getSovreign().getFullName());
        }

        public void processExpirables()
        {
            if (offensiveTarget != null && offensiveTarget.isGone()) { offensiveTarget = null; }

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
                    if (issue == null) { World.log(proposer.getFullName() + " elects not to submit a voting proposal on their turn");return; }

                    //Otherwise, on with voting for this new thing
                    voteSession = new VoteSession();
                    voteSession.issue = issue;
                    voteSession.timeRemaining = map.param.society_votingDuration;
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
                    double highestWeight = 0;
                    VoteOption bestChoice = null;
                    foreach (VoteOption option in voteSession.issue.options)
                    {
                        List<VoteMsg> msgs = new List<VoteMsg>();
                        double u = voteSession.issue.computeUtility(p, option, msgs);
                        option.msgs.Add(p, msgs);
                        if (u > highestWeight || bestChoice == null)
                        {
                            bestChoice = option;
                            highestWeight = u;
                        }}
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
                }
                
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
