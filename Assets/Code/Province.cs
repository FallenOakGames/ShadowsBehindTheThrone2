using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Province
    {
        public Hex coreHex;
        public string name;
        public List<Location> locations = new List<Location>();
        public Location capital;

        public float cr = (float)Eleven.random.NextDouble();
        public float cg = (float)Eleven.random.NextDouble();
        public float cb = (float)Eleven.random.NextDouble();

        public Province(Hex hex)
        {
            coreHex = hex;
        }
    }
}
