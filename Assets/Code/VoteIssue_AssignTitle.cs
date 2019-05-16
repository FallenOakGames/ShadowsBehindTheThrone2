using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_AssignTitle : VoteIssue
    {
        public Title_Land title;

        public VoteIssue_AssignTitle(Society soc,Title_Land title) : base(soc)
        {
            this.title = title;
        }

        public override double computeUtility(Person p, VotingOption option,List<VoteMsg> msgs)
        {
            double u = 0;

            double existingValue = 0;
            if (p.title_land != null)
            {
                existingValue = p.title_land.settlement.getPrestige();
            }

            double newValue = title.settlement.getPrestige();

            double benefitToPerson = newValue - existingValue;

            //We know how much they would be advantaged. We now need to know how much we like them to determine
            //if this is a good thing or not

            u = benefitToPerson * p.getRelation(option.person).value;
            msgs.Add(new VoteMsg("Benefit to " + option.person.getFullName(), u));

            return u;
        }

        public override void implement(VotingOption option)
        {
            throw new NotImplementedException();
        }
    }
}
