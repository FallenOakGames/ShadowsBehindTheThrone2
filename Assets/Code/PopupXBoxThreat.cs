using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupXBoxThreat : MonoBehaviour,PopupXScrollable
    {
        public Text mainText;
        public Text titleText;
        public ThreatItem item;
        public GameObject mover;
        public float targetX;


        public void Update()
        {
            Vector3 loc = new Vector3(targetX,mover.transform.position.y,  mover.transform.position.z);
            Vector3 delta = loc - mover.transform.position;
            if (delta.magnitude > 0.02f)
            {
                delta *= 0.075f;
            }
            mover.transform.Translate(delta);
        }

        public void setTo(ThreatItem item)
        {
            this.item = item;
            titleText.text = item.getTitle();
            if (item.p != null)
            {
                List<string> list = item.getReasons();
                string t = "Threat: " + (int)(item.threat);
                foreach (string s in list)
                {
                    t += "\n" + s;
                }

                t += "\n\nResponse: " + ThreatItem.responseNames[item.responseCode];
                mainText.text = t;
            }else
            {
                mainText.text = "Average Society-wide Threat Estimate:\n" + (int)(item.threat);
            }
        }

        public float xSize()
        {
            return 300;
        }

        public void setTargetX(float y)
        {
            targetX = y;
        }

        public string getTitle()
        {
            return item.getTitle();
        }

        public string getBody()
        {
            return "Each character evaluates the threats to their society independently. Their society then aggregates these together into a singular concensus of what to be afraid of."
                + "\nA society gains responses to its highest-rated threat, if this threat scores over 100. This occurs by allowing new types of vote for new types of action and by triggering zeitgeist events.";
        }
    }
}
