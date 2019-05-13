﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Globalist
    {
        public List<Trait> allTraits = new List<Trait>();
        public List<EconTrait> allEconTraits = new List<EconTrait>();
        public List<Property_Prototype> allProperties = new List<Property_Prototype>();
        public Dictionary<string, Property_Prototype> propertyMap = new Dictionary<string, Property_Prototype>();

        public EconTrait econTrait(string name)
        {
            foreach (EconTrait t in allEconTraits)
            {
                if (t.name == name)
                {
                    return t;
                }
            }
            throw new Exception("Unable to find econ trait: " + name);
        }
    }
}