using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIHex : MonoBehaviour
    {
        public UIMaster master;
        public Text title;
        public Text body;

        public void checkData()
        {
            Hex hex = GraphicalMap.selectedHex;
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
