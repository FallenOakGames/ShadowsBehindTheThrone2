using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class UIMidTop : MonoBehaviour
    {
        public UIMaster master;
        public Text powerText;
        

        public void Start()
        {
        }

        public void checkData()
        {
            powerText.text = "POWER: " + ((int)Math.Ceiling(master.world.map.overmind.power) + "/" + master.world.map.param.overmind_maxPower);
        }
    }
}
