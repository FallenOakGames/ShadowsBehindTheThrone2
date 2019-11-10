﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class PopupBoxVoteIssue : MonoBehaviour,PopupScrollable
    {
        public Text title;
        public Text body;
        public GameObject mover;
        public float targetY;
        public Society soc;
        public VoteIssue issue;


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
        

        public void setTo(Society soc,VoteIssue issue)
        {
            this.soc = soc;
            this.issue = issue;
            title.text = issue.ToString();
            //body.text = issue.getLargeDesc();
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
            soc.voteSession = new VoteSession();
            soc.voteSession.issue = issue;
            soc.voteSession.assignVoters();
        }

        public string getTitle()
        {
            return issue.ToString();
        }

        public string getBody()
        {
            return issue.getLargeDesc();
        }

        public bool overwriteSidebar()
        {
            return true;
        }

        public bool selectable()
        {
            return true;
        }
    }
}
