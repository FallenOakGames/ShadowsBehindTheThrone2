﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Title_Land
    {
        public Settlement settlement;
        public Person heldBy;
        public string titleM;
        public string titleF;

        public Title_Land(string titleM, string titleF)
        {
            this.titleM = titleM;
            this.titleF = titleF;
        }

        public string getName()
        {
            return "Hold of " + settlement.name;
        }
    }
}
