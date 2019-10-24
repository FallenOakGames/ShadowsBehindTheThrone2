using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_MilitaryAid : Property_Prototype
    {
        
        public Pr_MilitaryAid(Map map,string name) : base(map,name)
        {
            this.baseCharge = map.param.ability_militaryAidDur;
            this.milCapAdd = map.param.ability_militaryAidAmount;
            this.decaysOverTime = true;
        }

        public override void turnTick(Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_supply;
        }

        internal override string getDescription()
        {
            return "The description of a property would go here";
        }
    }
}
