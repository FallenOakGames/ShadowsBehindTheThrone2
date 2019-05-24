using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class SocialGroup
    {
        public Map map;
        private string name;
        public Color color;
        public Color color2;

        public Dictionary<SocialGroup, DipRel> relations = new Dictionary<SocialGroup, DipRel>();
        public DipRel selfRel;

        public double maxMilitary;
        public double currentMilitary;
        public int lastBattle;


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

        /*
         * By default, a social group is gone if it holds no territory. Can be overriden by specials
         */
        public virtual bool isGone() {
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this) { return false; }
            }
            return true;
        }

        public int getSize()
        {
            int reply = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this) { reply += 1; }
            }
            return reply;
        }

        public DipRel getRel(SocialGroup soc)
        {
            if (relations.ContainsKey(soc)) { return relations[soc]; }
            if (soc.relations.ContainsKey(this))
            {
                relations.Add(soc,soc.relations[this]);
                return relations[soc];
            }
            DipRel rel = new DipRel(map,this,soc);
            relations.Add(soc, rel);
            soc.relations.Add(this, rel);
            return rel;
        }

        public void setName(string newName)
        {
            name = newName;
        }

        public List<SocialGroup> getNeighbours()
        {
            List<SocialGroup> neighbours = new List<SocialGroup>();
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    foreach (Link l in loc.links)
                    {
                        if (l.other(loc).soc != this && l.other(loc).soc != null)
                        {
                            neighbours.Add(l.other(loc).soc);
                        }
                    }
                }
            }
            return neighbours;
        }

        public List<DipRel> getAllRelations()
        {
            List<DipRel> reply = new List<DipRel>();
            foreach (SocialGroup other in map.socialGroups)
            {
                reply.Add(getRel(other));
            }
            return reply;
        }

        public virtual string getName()
        {
            return name;
        }
        public virtual void turnTick()
        {
            processMilitaryCap();

            /*
            if (this.getSize() < 5)
            {
                int nWars = 0;
                foreach (SocialGroup other in getNeighbours())
                {
                    if (getRel(other).state == DipRel.dipState.war)
                    {
                        nWars += 1;
                    }

                }
                if (nWars == 0)
                {
                    int c = 0;
                    SocialGroup choice = null;
                    foreach (SocialGroup other in getNeighbours())
                    {
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            choice = other;
                        }
                    }
                    if (choice != null)
                    {
                        map.declareWar(this, choice);
                    }
                }
            }
            */
        }

        public void processMilitaryCap() { 
            maxMilitary = 0;
            double militaryRegen = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this && loc.settlement != null)
                {
                    militaryRegen += loc.settlement.militaryRegenAdd;
                    maxMilitary += loc.settlement.militaryCapAdd;
                }
            }
            maxMilitary = Math.Pow(maxMilitary, map.param.combat_maxMilitaryCapExponent);
            currentMilitary += militaryRegen;
            if (currentMilitary > maxMilitary) { currentMilitary = maxMilitary; }
        }
    }
}
