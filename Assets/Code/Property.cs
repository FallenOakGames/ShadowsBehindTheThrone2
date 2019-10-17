using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public abstract class Property
    {
        public abstract Location getLoc();
        public Property_Prototype proto;
        public int charge;
        public GraphicalProperty outer;
        public double militaryCapMult = 1;

        public Property(Property_Prototype proto)
        {
            this.proto = proto;
        }

        public virtual void endProperty(Location l)
        {
        }

        public static Property addProperty(Map map, Location location, string name)
        {
            if (map.globalist.propertyMap.ContainsKey(name) == false)
            {
                throw new Exception("Unable to find property named: " + name);
            }

            Property_Prototype proto = map.globalist.propertyMap[name];

            //Some, but not many, properties can be added multiply. Stackable defaults to false
            if (proto.stackable == false)
            {
                foreach (Property p in location.properties)
                {
                    if (p.proto.name != name) { continue; }
                    //Found matching name. Set timer to whatever's largest
                    p.charge = Math.Max(p.charge, proto.baseCharge);
                    return p;
                }
            }

            return null;
        }
    }
}
