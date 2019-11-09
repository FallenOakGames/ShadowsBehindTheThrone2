using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class Portrait : MonoBehaviour
    {
        public Text name;
        public Text info;
        public Text name2;
        public Text info2;

        public Image foreground;
        public Image background;

        // FIXME
        public Sprite personFrame;
        public Sprite personHead;
        public Sprite votesIcon;

        public void SetInfo(Person p)
        {
            foreground.sprite = personHead;
            background.sprite = personFrame;
            foreground.enabled = true;
            background.enabled = true;

            name.text  = p.getFullName();
            if (p.title_land != null)
                info.text = p.title_land.getName();
            else
                info.text = "Resident";

            info.text += "\n" + p.prestige + " Prestige";

            name2.text = "";
            info2.text = "";
        }

        public void SetInfo(Settlement s)
        {
            foreground.sprite = s.getSprite();
            foreground.enabled = true;
            background.enabled = false;

            name.text = s.name;
            if (s.title != null && s.title.heldBy != null)
                info.text = "Overseen by " + s.title.heldBy.getFullName();
            else
                info.text = "No Overseer";

            info.text += "\n" + s.getPrestige() + " Prestige";

            name2.text = "";
            info2.text = "";
        }

        public void SetInfo(VoteIssue i, VoteOption v)
        {
            foreground.sprite = votesIcon;
            foreground.enabled = false;
            background.enabled = false;

            name.text = "";
            info.text = "";

            name2.text = "For " + v.info(i, true);
            info2.text = v.votingWeight + " Influence";
            info2.text += "\n" + v.votesFor.Count + " Votes";
        }
    }
}
