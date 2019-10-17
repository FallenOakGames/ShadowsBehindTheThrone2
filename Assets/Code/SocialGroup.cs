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
        private string name;
        public Color color;
        public Color color2;

        public Dictionary<SocialGroup, DipRel> relations = new Dictionary<SocialGroup, DipRel>();
        public DipRel selfRel;

        public double threat_mult = 0;
        public double maxMilitary;
        public double currentMilitary;
        public double militaryRegen;
        public int lastBattle;
        private bool cachedGone;
        public double temporaryThreat;

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
        public virtual bool checkIsGone() {
            if (cachedGone) { return true; }
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this) {return false; }
            }
            cachedGone = true;
            return true;
        }
        /*
         * By default, a social group is gone if it holds no territory. Can be overriden by specials
         */
        public virtual bool isGone()
        {
            return cachedGone;
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
            return map.getNeighbours(this);
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
            temporaryThreat *= map.param.temporaryThreatDecay;
            computeMilitaryCap();
            processMilitaryRegen();

            
        }

        public virtual double getThreat(List<ReasonMsg> reasons)
        {
            ReasonMsg msg;
            double threat = currentMilitary + (maxMilitary/2);
            if (reasons != null)
            {
                msg = new ReasonMsg("Current Military", currentMilitary);
                reasons.Add(msg);
                msg = new ReasonMsg("Max Military", (maxMilitary/2));
                reasons.Add(msg);
            }

            if (this.threat_mult != 0)
            {
                int percent = (int)(100 * this.threat_mult);
                double addT = threat * this.threat_mult;
                threat += addT;
                if (reasons != null)
                {
                    msg = new ReasonMsg("+" + percent + "% from type", addT);
                    reasons.Add(msg);
                }

            }

            threat += temporaryThreat;
            if (reasons != null)
            {
                msg = new ReasonMsg("Temporary Threat", temporaryThreat);
                reasons.Add(msg);
            }
            return threat;
        }
        
        public void computeMilitaryCap() {
            maxMilitary = 0;
            militaryRegen = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this && loc.settlement != null)
                {
                    militaryRegen += loc.settlement.militaryRegenAdd;
                    maxMilitary += loc.settlement.getMilitaryCap() ;
                }
            }
            maxMilitary = Math.Pow(maxMilitary, map.param.combat_maxMilitaryCapExponent);
            militaryRegen = Math.Pow(militaryRegen, map.param.combat_maxMilitaryCapExponent);
        }

        public void processMilitaryRegen() { 
            currentMilitary += militaryRegen;
            if (currentMilitary > maxMilitary) { currentMilitary = maxMilitary; }
        }

        public virtual void takeLocationFromOther(SocialGroup def, Location taken)
        {
            this.temporaryThreat += map.param.threat_takeLocation;
        }
    }
}
