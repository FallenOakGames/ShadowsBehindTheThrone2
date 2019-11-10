﻿using UnityEngine;
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
            VoteIssue issue = null;

            List<VoteIssue> potentialIssues = new List<VoteIssue>();
            if (soc.posture == Society.militaryPosture.offensive && soc.offensiveTarget != null)
            {
                issue = new VoteIssue_DeclareWar(soc, soc.offensiveTarget, proposer);
                potentialIssues.Add(issue);
                VoteOption option_0 = new VoteOption();
                option_0.index = 0;
                issue.options.Add(option_0);
                VoteOption option_1 = new VoteOption();
                option_1.index = 1;
                issue.options.Add(option_1);
            }

            
            if (proposer.title_land != null)
            {
                potentialIssues.AddRange(econIssues(map, proposer, soc));
            }

            issue = new VoteIssue_MilitaryStance(soc, proposer);
            potentialIssues.Add(issue);
            for (int i = 0; i < 3; i++)
            {
                VoteOption opt = new VoteOption();
                opt.index = i;
                issue.options.Add(opt);
            }

            //Check to see if you want to alter offensive military targetting
            issue = new VoteIssue_SetOffensiveTarget(soc, proposer);
            foreach (SocialGroup neighbour in map.getExtendedNeighbours(soc))
            {
                VoteOption option = new VoteOption();
                option.group = neighbour;
                issue.options.Add(option);
            }
            potentialIssues.Add(issue);

            //Check to see if you want to alter defensive military targetting
            issue = new VoteIssue_SetDefensiveTarget(soc, proposer);
            foreach (ThreatItem item in proposer.threatEvaluations)
            {
                if (item.group == null) { continue; }
                VoteOption option = new VoteOption();
                option.group = item.group;
                issue.options.Add(option);
            }

            foreach (Title t in soc.titles)
            {
                if (t.turnLastAssigned - map.turn > map.param.society_minTimeBetweenTitleReassignments)
                {
                    issue = new VoteIssue_AssignTitle(soc, proposer, t);
                    potentialIssues.Add(issue);
                    //Everyone is eligible
                    foreach (Person p in soc.people)
                    {
                        VoteOption opt = new VoteOption();
                        opt.person = p;
                        issue.options.Add(opt);
                    }
                }
            }
            foreach (Location loc in map.locations)
            {
                if (loc.soc == soc && loc.settlement != null && loc.settlement.title != null)
                {
                    issue = new VoteIssue_AssignLandedTitle(soc, proposer, loc.settlement.title);
                    potentialIssues.Add(issue);
                    //Everyone is eligible
                    foreach (Person p in soc.people)
                    {
                        VoteOption opt = new VoteOption();
                        opt.person = p;
                        issue.options.Add(opt);
                    }
                }
            }

            map.world.ui.addBlocker(map.world.prefabStore.getScrollSet(soc,potentialIssues).gameObject);
        }

        public List<VoteIssue> econIssues(Map map,Person p,Society society)
        {

            List<VoteIssue> reply = new List<VoteIssue>();
            List<EconTrait> all = new List<EconTrait>();
            List<EconTrait> mine = new List<EconTrait>();

            foreach (EconTrait trait in p.title_land.settlement.econTraits())
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
                    VoteIssue issue = new VoteIssue_EconomicRebalancing(society, p, econ_from, econ_to);
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

                    reply.Add(issue);
                }
            }
            return reply;
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