﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class SocialGroup
    {
        public Map map;
        public string name;
        public Color color;
        public Color color2;

        public Dictionary<SocialGroup, DipRel> relations = new Dictionary<SocialGroup, DipRel>();
        public DipRel selfRel;



        public SocialGroup(Map map)
        {
            this.map = map;
            color = new Color(
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble());
            color2 = new Color(
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble());
            name = "SocialGroup";

            //Self-diplomacy
            DipRel rel = new DipRel(map, this, this);
            selfRel = rel;
            relations.Add(this, rel);
        }

        public virtual bool isGone() { return false; }

        public virtual void turnTick()
        {

        }
    }
}
