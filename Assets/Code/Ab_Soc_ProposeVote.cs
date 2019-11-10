using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Assets.Code
{
    public class Ab_Soc_ProposeVote : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = map.overmind.enthralled.society;

            Person proposer = map.overmind.enthralled;

            List<VoteIssue> potentialIssues = new List<VoteIssue>();
            if (soc.posture == Society.militaryPosture.offensive && soc.offensiveTarget != null)
            {
                VoteIssue_DeclareWar war = new VoteIssue_DeclareWar(soc, soc.offensiveTarget, proposer);
                potentialIssues.Add(war);
            }

            VoteIssue issue = new VoteIssue_MilitaryStance(soc,proposer);
            potentialIssues.Add(issue);

            foreach (Title t in soc.titles)
            {
                if (t.turnLastAssigned - map.turn > map.param.society_minTimeBetweenTitleReassignments)
                {
                    issue = new VoteIssue_AssignTitle(soc, proposer, t);
                    potentialIssues.Add(issue);
                }
            }
            foreach (Location loc in map.locations)
            {
                if (loc.soc == soc && loc.settlement != null && loc.settlement.title != null)
                {
                    issue = new VoteIssue_AssignLandedTitle(soc, proposer, loc.settlement.title);
                    potentialIssues.Add(issue);
                }
            }

            map.world.prefabStore.getScrollSet(soc,potentialIssues);
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled.society.voteSession != null) { return false; }

            return true;
        }

        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Proposes a vote to the society, allowing you to get the society to choose to act in a certain way (if you can convince enough of the voters)."
                + "\n[Requires an enthralled noble and a society not currently voting]";
        }

        public override string getName()
        {
            return "Propose Vote";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}