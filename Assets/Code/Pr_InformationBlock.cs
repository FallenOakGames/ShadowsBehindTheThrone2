using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_InformationBlock : Property_Prototype
    {
        
        public Pr_InformationBlock(Map map,string name) : base(map,name)
        {
            this.informationAvailabilityMult = 0.5;
        }

        public override void turnTick(Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_lookingGlass;
        }

        internal override string getDescription()
        {
            return "The description of a property would go here";
        }
    }
}
