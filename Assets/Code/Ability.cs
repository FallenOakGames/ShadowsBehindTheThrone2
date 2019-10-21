using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public abstract class Ability
    {
        public abstract string getName();
        public abstract string getDesc();
        public abstract int getCost();
        public virtual void cast(Map map, Hex hex) { }
        public abstract bool castable(Map map, Hex hex);
        public abstract Sprite getSprite(Map map);
        public virtual string specialCost() { return null; }
    }
}