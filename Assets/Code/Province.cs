using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Province
    {
        public Map map;
        public int coreX;
        public int coreY;
        public Hex coreHex { get { return map.grid[coreX][coreY]; } }
        public string name;
        public List<Location> locations = new List<Location>();
        public Location capital;
        public List<EconTrait> econTraits = new List<EconTrait>();
        public bool isSea = false;

        public float cr = (float)Eleven.random.NextDouble();
        public float cg = (float)Eleven.random.NextDouble();
        public float cb = (float)Eleven.random.NextDouble();

        public Province(Map map,Hex hex)
        {
            this.map = map;
            coreX = hex.x;
            coreY = hex.y;
        }
    }
}
