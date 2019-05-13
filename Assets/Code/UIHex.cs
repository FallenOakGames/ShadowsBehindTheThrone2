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
            if (GraphicalMap.selectedHex == null)
            {
                title.text = "";
                body.text = "";
            }
            else
            {
                title.text = GraphicalMap.selectedHex.getName();
               string bodyText = "Body text for hex " + GraphicalMap.selectedHex.getName();

                bodyText += "\nAttachedTo " + GraphicalMap.selectedHex.territoryOf.hex.getName();
                bodyText += "\nProvince: " + GraphicalMap.selectedHex.province.name;
                body.text = bodyText;
            }
        }
    }
}
