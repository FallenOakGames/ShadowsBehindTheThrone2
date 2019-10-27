using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public abstract class Ability
    {
        public abstract string getName();
        public abstract string getDesc();
        public abstract int getCost();
        public abstract bool castable(Map map, Hex hex);
        public abstract Sprite getSprite(Map map);
        public virtual string specialCost() { return null; }

        public virtual void cast(Map map, Hex hex) {
            map.overmind.power -= getCost();
            if (map.param.overmind_singleAbilityPerTurn) { map.overmind.hasTakenAction = true; }
            World.log("Cast " + this.ToString() + " " + this.getName());
        }
    }
}