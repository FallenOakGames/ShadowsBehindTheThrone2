using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Person
    {
        public string firstName;
        public bool male = Eleven.random.Next(2) == 0;
        public List<Title> titles = new List<Title>();
        public TitleLanded title_land;
        public Society society;
        public Dictionary<Person, RelObj> relations = new Dictionary<Person, RelObj>();
        public double prestige = 1;
        public int lastVoteProposalTurn;
        public VoteIssue lastProposedIssue;
        
        public double politics_militarism = Eleven.random.NextDouble() * 2 - 1;

        public enum personState { normal,enthralled,broken};
        public personState state = personState.normal;

        public Person(Society soc)
        {
            this.society = soc;
            firstName = TextStore.getName(male);
            getRelation(this).setLiking(100);//Set self-relation to 100
        }

        public void turnTick()
        {
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
            if (relations.ContainsKey(other))
            {
                return relations[other];
            }
            RelObj rel = new RelObj(this, other, map.param.relObj_defaultLiking);
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
            if (male)
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
        }

        public VoteIssue proposeVotingIssue()
        {
            //Note we start at 1 utility. This means no guaranteed negative/near-zero utility options will be proposed
            double bestU = 1;
            VoteIssue bestIssue = null;

            bool existFreeTitles = false;


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
                if (t.heldBy != null) { continue; }
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
                    double localU = issue.computeUtility(this, opt, new List<VoteMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU)
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                }
            }

            if (society.getSovreign() != null)
            {
                foreach (Location loc in map.locations)
                {
                    //If there are unhanded out titles, only consider those. Else, check all.
                    //Maybe they could be rearranged (handed out or simply swapped) in a way which could benefit you
                    if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && ((!existFreeTitles) || (loc.settlement.title.heldBy == null)))
                    {
                        issue = new VoteIssue_AssignLandedTitle(society, this, loc.settlement.title);
                        if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
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
                            double localU = issue.computeUtility(this, opt, new List<VoteMsg>()) * Eleven.random.NextDouble();
                            if (localU > bestU)
                            {
                                bestU = localU;
                                bestIssue = issue;
                            }
                        }
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
                            if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

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
                                double localU = issue.computeUtility(this, opt, new List<VoteMsg>()) * Eleven.random.NextDouble();
                                if (localU > bestU)
                                {
                                    bestU = localU;
                                    bestIssue = issue;
                                }
                            }

                        }
                    }
                }

                //Check to see if you want to alter military targetting
                issue = new VoteIssue_SetOffensiveTarget(society, this);
                foreach (SocialGroup neighbour in society.getNeighbours())
                {
                    VoteOption option = new VoteOption();
                    option.group = neighbour;
                    issue.options.Add(option);
                }
                foreach (VoteOption opt in issue.options)
                {
                    if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip

                    //Random factor to prevent them all rushing a singular voting choice
                    double localU = issue.computeUtility(this, opt, new List<VoteMsg>()) * Eleven.random.NextDouble();
                    if (localU > bestU)
                    {
                        bestU = localU;
                        bestIssue = issue;
                    }
                }

                //Check to see if you want to declare war
                if (society.offensiveTarget != null && society.getRel(society.offensiveTarget).state != DipRel.dipState.war)
                {
                    issue = new VoteIssue_DeclareWar(society, society.offensiveTarget, this);
                    VoteOption option = new VoteOption();
                    option.index = 0;
                    issue.options.Add(option);

                    option = new VoteOption();
                    option.index = 1;
                    issue.options.Add(option);

                    foreach (VoteOption opt in issue.options)
                    {
                        if (lastProposedIssue != null && lastProposedIssue.GetType() == issue.GetType()) { break; }//Already seen this proposal, most likely. Make another or skip
                                                                                                                   //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, opt, new List<VoteMsg>()) * Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                        }
                    }
                }
            }

            if (bestIssue != null)
            {
                World.log(this.getFullName() + " returning voting issue " + bestIssue.ToString() + " with expected utility " + bestU);
            }
            lastProposedIssue = bestIssue;
            return bestIssue;
        }
    }
}
