using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxAbility : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public Text cost;
        public Image icon;
        public GameObject mover;
        public float targetY;
        public Ability ability;
        public Hex hex;


        public void Update()
        {
            Vector3 loc = new Vector3(mover.transform.position.x, targetY, mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        public void setTo(Ability a,Hex hex)
        {
            ability = a;
            this.hex = hex;
            title.text = a.getName();
            body.text = a.getDesc();
            cost.text = ""+a.getCost();
            icon.sprite = a.getSprite(hex.map);
        }

        public float ySize()
        {
            return 100;
        }

        public void setTargetY(float y)
        {
            targetY = y;
        }
        public void clicked(Map map)
        {
            //selector.selected(person,agent);
            ability.cast(map, hex);
        }

        public string getTitle()
        {
            //return person.getFullName();
            return ability.getName();
        }

        public string getBody()
        {
            string reply = "Ability selection.";
            reply += "\n\nAbilities are performed on locations. Actions have restrictions on which locations are valid targets.";
            reply += "\n\nYou may cast any ability as long as you have more than zero power. ";
                reply += "Your power may go negative, allowing you to perform an expensive in an emergency, but you will then have a negative power period.";
            return reply;
        }

        public bool overwriteSidebar()
        {
            return true;
        }
    }
}
