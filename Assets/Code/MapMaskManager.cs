﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    //[Serializable,HideInInspector]
    public class MapMaskManager
    {
        public enum maskType { NONE, NATION, PROVINCE, INFORMATION,THREAT,LIKING_FROM,LIKING_TO,TESTING };
        public maskType mask = maskType.NONE;
        public Map map;
        [NonSerialized]
        private Color invis = new Color(0, 0, 0, 0);
        [NonSerialized]
        private Color dark = new Color(0, 0, 0, 0.8f);

        public MapMaskManager(Map map)
        {
            this.map = map;
        }

        public bool applyMask(Hex hex)
        {
            return mask != maskType.NONE;
        }

        public Color getColor(Hex hex)
        {
            if (mask == maskType.NATION)
            {
                if (hex.owner != null)
                {
                    float r = hex.owner.color.r;
                    float g = hex.owner.color.g;
                    float b = hex.owner.color.b;
                    Color color = new Color(r, g, b, 0.8f);
                    return color;
                }
                else
                {
                    return new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }
            }
            else if (mask == maskType.PROVINCE)
            {
                return new Color(hex.province.cr, hex.province.cg, hex.province.cb, 0.5f);
            }
            else if (mask == maskType.INFORMATION)
            {
                if (hex.location != null && GraphicalMap.selectedHex != null && GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.soc != null)
                {
                    SocialGroup group = GraphicalMap.selectedHex.location.soc;

                    float mult = (float)map.getInformationAvailability(hex.location, group);
                    mult = Mathf.Max(0, mult);
                    mult = Mathf.Min(1, mult);
                    float r = mult;
                    float g = mult;
                    float b = mult;

                    //Color color = new Color(r, g, b, 0.8f);
                    Color color = new Color(0,0,0,(1-mult));
                    return color;
                }
                else if (hex.location != null)
                {
                    return new Color(0f, 0f, 0f, 1f);
                }
                else
                {
                    return new Color(0f, 0f, 0f, 0.75f);
                }
            }
            else if (mask == maskType.THREAT)
            {
                if (hex.location != null && GraphicalMap.selectedHex != null && GraphicalMap.selectedHex.location != null && GraphicalMap.selectedHex.location.settlement != null && GraphicalMap.selectedHex.location.settlement.title != null && GraphicalMap.selectedHex.location.settlement.title.heldBy != null)
                {
                    SocialGroup group = GraphicalMap.selectedHex.location.soc;

                    Person p = GraphicalMap.selectedHex.settlement.title.heldBy;
                    double highestThreat = 0;
                    foreach (ThreatItem item in p.threatEvaluations)
                    {
                        if (item.group != null && item.threat > highestThreat)
                        {
                            highestThreat = item.threat;
                        }
                    }
                    if (highestThreat != 0)
                    {
                        if (hex.location.soc == null)
                        {
                            return Color.black;
                        }
                        double threat = 0;
                        foreach (ThreatItem item in p.threatEvaluations)
                        {
                            if (item.group == hex.location.soc) { threat = item.threat/highestThreat;break; }
                        }
                        float mult = (float)threat;
                        mult = Mathf.Max(0, mult);
                        mult = Mathf.Min(1, mult);
                        float r = mult;
                        float g = mult;
                        float b = mult;

                        //Color color = new Color(r, g, b, 0.8f);
                        Color color = new Color(mult, 0, 0, 0.5f);
                        return color;
                    }
                    else
                    {
                        return Color.black;
                    }
                }
                else if (hex.location != null)
                {
                    return new Color(0f, 0f, 0f, 1f);
                }
                else
                {
                    return new Color(0f, 0f, 0f, 0.75f);
                }
            }

            else if (mask == maskType.LIKING_FROM)
            {
                Color c = new Color(0, 0, 0, 0.5f);
                try
                {
                    Person me = GraphicalMap.selectedHex.location.settlement.title.heldBy;
                    Person them = hex.location.settlement.title.heldBy;
                    float liking = (float)me.getRelation(them).getLiking();
                    if (liking > 0)
                    {
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(0, liking, 0, 0.5f);
                    }
                    else
                    {
                        liking *= liking;
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(liking, 0, 0, 0.5f);

                    }
                }
                catch (NullReferenceException e)
                {

                }
                catch (ArgumentNullException e)
                {

                }
                return c;
            }

            else if (mask == maskType.LIKING_TO)
            {
                Color c = new Color(0, 0, 0, 0.5f);
                try
                {
                    Person me = GraphicalMap.selectedHex.location.settlement.title.heldBy;
                    Person them = hex.location.settlement.title.heldBy;
                    float liking = (float)them.getRelation(me).getLiking();
                    if (liking > 0)
                    {
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(0, liking, 0, 0.5f);
                    }
                    else
                    {
                        liking *= liking;
                        if (liking > 100) { liking = 100; }
                        liking /= 100;
                        c = new Color(liking, 0, 0, 0.5f);

                    }
                }
                catch (NullReferenceException e)
                {

                }
                catch (ArgumentNullException e)
                {

                }
                return c;
            }
            else if (mask == maskType.TESTING)
            {
                if (hex.location != null)
                {
                    if (hex.location.debugVal == map.turn)
                    {
                        return new Color(1, 1, 1, 0.5f);
                    }
                    else
                    {
                        return new Color(0, 0, 0, 0.5f);

                    }
                }

                return new Color(0f, 0f, 0f, 0.75f);
            }
            else if (mask == maskType.TESTING)
            {
                return new Color(0f, 0f, 0f, 0.75f);
            }
            else
            {
                return new Color(0, 0, 0, 0);
            }
        }

        public void setMask(maskType type)
        {
            mask = type;
        }
        public void toggleMask(maskType type)
        {
            if (mask == type) { mask = maskType.NONE; }
            else { mask = type; }
        }
    }
}
