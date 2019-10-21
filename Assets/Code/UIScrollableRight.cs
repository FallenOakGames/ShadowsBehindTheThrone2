using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIScrollableRight : MonoBehaviour
    {
        public UIMaster master;
        public Text title;
        //public Text body;

        public GameObject portraitPrefab;
        public GameObject testButtonObj;
        public RectTransform listContent;

        public Toggle bPeople;
        public Toggle bPlaces;
        public Toggle bVotes;
        public Toggle bActions;

        public enum Tab { People, Places, Votes, Actions };
        private Tab currentTab;

        public void Start()
        {
            currentTab = Tab.People;
        }

        int nCalls;
        public void checkData()
        {
            title.text = "";

            // FIXME: check if this needs to be done?
            foreach (Transform t in listContent)
            {
                GameObject.Destroy(t.gameObject);
            }

            if (currentTab == Tab.Actions)
            {
                fillActionsInTab();
                return;
            }
            Society soc = getSociety(GraphicalMap.selectedHex);
            if (soc == null) return;

            //title.text = soc.getName();
            switch (currentTab)
            {
                case Tab.People:
                    {
                        foreach (Person p in soc.people)
                        {
                            GameObject pp = Instantiate(portraitPrefab, listContent);
                            pp.GetComponent<Portrait>().SetInfo(p);
                        }

                        break;
                    }
                case Tab.Places:
                    {
                        foreach (Settlement s in getSettlements(soc))
                        {
                            GameObject sp = Instantiate(portraitPrefab, listContent);
                            sp.GetComponent<Portrait>().SetInfo(s);
                        }

                        break;
                    }
                case Tab.Votes:
                    {
                        if (soc.voteSession != null)
                        {
                            List<VoteOption> vs = soc.voteSession.issue.options;
                            foreach (Person p in soc.people)
                            {
                                double highestWeight = 0;
                                VoteOption bestChoice = null;
                                foreach (VoteOption option in vs)
                                {
                                    List<ReasonMsg> msgs = new List<ReasonMsg>();
                                    double u = soc.voteSession.issue.computeUtility(p, option, msgs);
                                    if (u > highestWeight || bestChoice == null)
                                    {
                                        bestChoice = option;
                                        highestWeight = u;
                                    }
                                }
                                bestChoice.votingWeight += p.prestige;
                            }

                            foreach (VoteOption v in vs)
                            {
                                GameObject vp = Instantiate(portraitPrefab, listContent);
                                vp.GetComponent<Portrait>().SetInfo(v);

                                v.votingWeight = 0.0;
                            }
                        }
                        break;
                    }

                case Tab.Actions:
                    {
                        break;
                    }
            }
        }

        public void fillActionsInTab()
        {
            for (int i = 0; i < 3; i++)
            {
                GameObject sp = Instantiate(testButtonObj, listContent);
                Button script = sp.GetComponent<Button>();
                script.onClick.AddListener(delegate { bTestClick(i); });
            }
        }
        public void bTestClick(int i)
        {
            World.log("Received data " + i);
        }

        public void onToggle(bool b)
        {
            if (bPeople.isOn) currentTab = Tab.People;
            else if (bPlaces.isOn) currentTab = Tab.Places;
            else if (bVotes.isOn) currentTab = Tab.Votes;
            else if (bActions.isOn) currentTab = Tab.Actions;

            checkData();
        }

        private Society getSociety(Hex h)
        {
            if (h == null) return null;
            if (h.location == null) return null;
            if (h.location.soc == null) return null;
            if (!(h.location.soc is Society)) return null;

            return (Society)h.location.soc;
        }

        private List<Settlement> getSettlements(Society s)
        {
            // FIXME: LINQ :(
            return s.map.locations
                .Where(l => l.soc == s && l.settlement != null)
                .Select(l => l.settlement)
                .ToList();
        }

        public void checkData_old()
        {
            Hex hex = GraphicalMap.selectedHex;
            if (hex == null)
            {
                title.text = "";
                //body.text = "";
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
                                bodyText += "\n   -" + p.getFullName();
                            }

                            if (locSoc.offensiveTarget != null)
                            {
                                bodyText += "\nOffensive: " + locSoc.offensiveTarget.getName();
                            }
                            else
                            {
                                bodyText += "\nOffensive: None";
                            }

                        }
                    }
                }
                foreach (EconTrait t in hex.province.econTraits)
                {
                    bodyText += "\n  Industry: " + t.name;
                }
                //body.text = bodyText;
            }
        }
    }
}
