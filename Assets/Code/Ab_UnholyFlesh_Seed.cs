using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class Ab_UnholyFlesh_Seed : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            map.socialGroups.Add(new SG_UnholyFlesh(map, hex.location));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc != null) { return false; }
            if (hex.location.settlement != null) { return false; }
            if (hex.location.isOcean) { return false; }
            return true;
        }

        public override int getCost()
        {
            return 12;
        }

        public override string getDesc()
        {
            return "Creates a seed from which the unholy flesh can erupt. Violent and horrifying, the flesh is unsubtle and obvious, and will quickly attract the attention of the nations of men."
                 + "\nCan be expanded with further abilities once seeded.";
        }

        public override string getName()
        {
            return "Seed the flesh";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_ghoul;
        }
    }
}