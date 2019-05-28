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
        public Title_Land title_land;
        public Society society;
        public Dictionary<Person, RelObj> relations = new Dictionary<Person, RelObj>();
        public double prestige = 1;
        public int lastVoteProposalTurn;

        public double politics_militarism = Eleven.random.NextDouble() * 2 - 1;

        public enum personState { normal,enthralled,broken};
        public personState state = personState.normal;

        public Person(Society soc)
        {
            this.society = soc;
            firstName = TextStore.getName(male);
            getRelation(this).value = 100;//Set self-relation to 100
        }

        public void turnTick()
        {
            double targetPrestige = map.param.person_defaultPrestige;
            if (title_land != null)
            {
                targetPrestige += title_land.settlement.basePrestige;
            }
            if (Math.Abs(prestige-targetPrestige) < map.param.person_prestigeDeltaPerTurn)
            {
                prestige = targetPrestige;
            }
            else if (prestige < targetPrestige) { prestige += map.param.person_prestigeDeltaPerTurn; }
            else if (prestige > targetPrestige) { prestige -= map.param.person_prestigeDeltaPerTurn; }
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
            RelObj rel = new RelObj(this, other,map.param.relObj_defaultLiking);
            relations.Add(other, rel);
            return rel;
        }

        public string getTitles()
        {
            if (male)
            {
                if (title_land != null) { return title_land.titleM; }
                return "Lord";
            }else
            {
                if (title_land != null) { return title_land.titleF; }
                return "Lady";
            }
        }

        public void die(string v)
        {
            World.log(v);
        }

        public VoteIssue proposeVotingIssue()
        {
            //Note we start at zero utility. This means no guaranteed negative utility options will be proposed
            double bestU = 0;
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
            foreach (Location loc in map.locations)
            {
                //If there are unhanded out titles, only consider those. Else, check all.
                //Maybe they could be rearranged (handed out or simply swapped) in a way which could benefit you
                if (loc.soc == society && loc.settlement != null && loc.settlement.title != null && ((!existFreeTitles) || (loc.settlement.title.heldBy == null)))
                {
                    issue = new VoteIssue_AssignTitle(society,loc.settlement.title);
                    //Everyone is eligible
                    foreach (Person p in society.people)
                    {
                        VotingOption opt = new VotingOption();
                        opt.person = p;
                        issue.options.Add(opt);
                    }

                    foreach (VotingOption opt in issue.options)
                    {
                        //Random factor to prevent them all rushing a singular voting choice
                        double localU = issue.computeUtility(this, opt, new List<VoteMsg>())*Eleven.random.NextDouble();
                        if (localU > bestU)
                        {
                            bestU = localU;
                            bestIssue = issue;
                        }
                    }
                }
            }

            //Change current offensive target
            issue = new VoteIssue_SetOffensiveTarget(society);
            foreach (SocialGroup group in society.getNeighbours())
            {
            }

            if (bestIssue != null)
            {
                World.log(this.getFullName() + " returning voting issue " + bestIssue.ToString() + " with expected utility " + bestU);
            }
            return bestIssue;
        }
    }
}
