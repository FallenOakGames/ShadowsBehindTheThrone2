﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxVote : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public GameObject mover;
        public float targetY;
        public bool usable = true;
        public Image background;
        public VoteOption option;
        public VoteSession sess;

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

        public void setTo(VoteSession session,VoteOption option)
        {
            title.text = session.issue.ToString();
            body.text = option.info(session.issue);
            this.option = option;
            this.sess = session;
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
        }

        public string getTitle()
        {
            return option.info();
        }

        public string getBody()
        {
            string reply = "Voting.";

            return reply;
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            return usable;
        }
    }
}
