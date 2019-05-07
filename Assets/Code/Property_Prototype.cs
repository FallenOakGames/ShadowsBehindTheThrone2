using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Property_Prototype
    {
        public string name;

        public int baseCharge = 10;
        public bool displayCharge = false;
        public bool decaysOverTime = false;
        public bool deleteOnZeroTimeRemaining = false;
        public bool stackable = false;

        public Property_Prototype(string name)
        {
            this.name = name;
        }
        public virtual void turnTick(Location location)
        {

        }
    }
}
