using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UILeftPrimary : MonoBehaviour
    {
        public UIMaster master;
        public Text title;
        public Text body;
        public Text maskTitle;
        public Text maskBody;
        public Text abilityButtonText;


        public void Start()
        {
        }

        private Society getSociety(Hex h)
        {
            if (h == null) return null;
            if (h.location == null) return null;
            if (h.location.soc == null) return null;
            if (!(h.location.soc is Society)) return null;

            return (Society)h.location.soc;
        }

        public void checkData()
        {
            Hex hex = GraphicalMap.selectedHex;

            abilityButtonText.text = "Use Ability (" + master.world.map.overmind.countAvailableAbilities(hex) + ")";
            
            maskTitle.text = GraphicalMap.map.masker.getTitleText();
            maskBody.text = GraphicalMap.map.masker.getBodyText();
            if (hex == null)
            {
                title.text = "";
                body.text = "";
            }
            else
            {
                title.text = GraphicalMap.selectedHex.getName();
               string bodyText = "Body text for hex " + GraphicalMap.selectedHex.getName();

                bodyText += "\nAttachedTo " + GraphicalMap.selectedHex.territoryOf.hex.getName();
                bodyText += "\nProvince: " + hex.province.name;
                if (hex.location != null)
                {
                    if (hex.location.settlement != null)
                    {
                        if (hex.location.settlement.title != null)
                        {
                            if (hex.location.settlement.title.heldBy != null)
                            {
                                bodyText += "\nTitle held by: " + hex.location.settlement.title.heldBy.getFullName();
                            }
                            else
                            {
                                bodyText += "\nTitle currently unheld";

                            }
                        }
                    }

                    if (hex.location.soc != null)
                    {
                        bodyText += "\nSocial group: " + hex.location.soc.getName();
                        if (hex.location.soc is Society)
                        {
                            Society locSoc = (Society)hex.location.soc;

                            foreach (EconEffect effect in locSoc.econEffects)
                            {
                                bodyText += "\nEcon from " + effect.from.name + " to " + effect.to.name;
                            }

                            foreach (Person p in locSoc.people)
                            {
                                //bodyText += "\n   -" + p.getFullName();
                            }

                            bodyText += "\nMILITARY POSTURE: " + locSoc.posture;
                            if (locSoc.offensiveTarget != null)
                            {
                                bodyText += "\nOffensive: " + locSoc.offensiveTarget.getName();
                            }
                            else
                            {
                                bodyText += "\nOffensive: None";
                            }
                            if (locSoc.defensiveTarget != null)
                            {
                                bodyText += "\nDefensive: " + locSoc.defensiveTarget.getName();
                            }
                            else
                            {
                                bodyText += "\nDefensive: None";
                            }
                            bodyText += "\nRebel cap " + locSoc.data_rebelLordsCap;
                            bodyText += "\nLoyal cap " + locSoc.data_loyalLordsCap;

                        }

                        List<ReasonMsg> msgs = new List<ReasonMsg>();
                        double threat = hex.location.soc.getThreat(msgs);
                        bodyText += "\nThreat: " + threat;
                        foreach (ReasonMsg msg in msgs)
                        {
                            bodyText += "\n   " + msg.msg + " " + msg.value;
                        }
                    }
                }
                foreach (EconTrait t in hex.province.econTraits)
                {
                    bodyText += "\n  Industry: " + t.name;
                }
                body.text = bodyText;
            }
        }
    }
}
