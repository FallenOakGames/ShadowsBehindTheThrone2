﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_AssignTitle : VoteIssue
    {
        public Title title;

        public VoteIssue_AssignTitle(Society soc,Person proposer,Title title) : base(soc,proposer)
        {
            this.title = title;
        }

        public override string ToString()
        {
            string add = ""; 
            if (title.heldBy != null)
            {
                add = " (" + title.heldBy.getFullName() + ")";
            }
            else
            {
                add = " (unassigned)";
            }
            return "VoteIssue\"Assign " + title.getName() + "\" " + add;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            Person p = option.person;
            double existingValue = 0;
            if (p.title_land != null)
            {
                existingValue = p.title_land.settlement.getPrestige();
            }

            double newValue = title.getPrestige();

            double benefitToPerson = newValue - existingValue;

            //We know how much they would be advantaged. We now need to know how much we like them to determine
            //if this is a good thing or not

            double localU = benefitToPerson * voter.getRelation(p).getLiking();
            msgs.Add(new ReasonMsg("Benefit to " + p.getFullName(), localU));
            u += localU;

            //We need to know if someone's going to lose out here
            //(Note this is irrelevant if they're the person who's being voted on)
            if (title.heldBy != null && title.heldBy != p)
            {
                double damageToOther = title.getPrestige();
                localU = -damageToOther * voter.getRelation(title.heldBy).getLiking();
                msgs.Add(new ReasonMsg("Harm to " + title.heldBy.getFullName(), localU));
                u += localU;
            }
            

            return u;
        }

        public override void implement(VoteOption option)
        {
            if (society.people.Contains(option.person) == false) { World.log("Invalid option. Person cannot hold title."); return; }
            base.implement(option);
            if (title.heldBy == option.person)
            {
                World.log("Title: " + title.getName() + " remains held by " + option.person.getFullName());
            }
            
            //Title already has a person
            if (title.heldBy != null)
            {
                World.log(title.heldBy.getFullName() + " is losing title " + title.getName());
                title.heldBy.titles.Remove(title);
                title.heldBy = null;
            }

            World.log(option.person.getFullName() + " has been granded the title of " + title.getName());
            title.heldBy = option.person;
            option.person.titles.Add(title);

            society.turnSovreignAssigned = society.map.turn;
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
