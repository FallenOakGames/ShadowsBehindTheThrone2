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

        public Image foreground;
        public Image background;

        // FIXME
        public Sprite personFrame;
        public Sprite personHead;

        public void SetInfo(Person p)
        {
            foreground.sprite = personHead;
            background.sprite = personFrame;
            background.enabled = true;

            name.text  = p.getFullName();
            if (p.title_land != null)
                info.text = p.title_land.getName();
            else
                info.text = "Resident";

            info.text += "\n" + p.prestige + " Prestige";
        }

        public void SetInfo(Settlement s)
        {
            foreground.sprite = s.getSprite();
            background.sprite = null;
            background.enabled = false;

            name.text = s.name;
            if (s.title != null && s.title.heldBy != null)
                info.text = "Overseen by " + s.title.heldBy.getFullName();
            else
                info.text = "No Overseer";

            info.text += "\n" + s.getPrestige() + " Prestige";
        }

        public void SetInfo(VoteOption v)
        {
            foreground.sprite = personHead;
            background.sprite = personFrame;
            background.enabled = true;

            name.text = "For " + v.person.getFullName();
            info.text = v.votingWeight + " Influence";
        }
    }
}
