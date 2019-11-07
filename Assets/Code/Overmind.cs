using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Overmind
    {
        public float power;
        public bool hasTakenAction;

        public List<Ability> abilities = new List<Ability>();
        public Map map;
        public Person enthralled;

        public Overmind(Map map)
        {
            this.map = map;
            abilities.Add(new Ab_Enth_Enthrall());

            //abilities.Add(new Ab_TestAddShadow());

            abilities.Add(new Ab_Fishman_Lair());
            abilities.Add(new Ab_Fishman_Call());
            abilities.Add(new Ab_Fishman_Attack());
            abilities.Add(new Ab_Enth_MiliaryAid());
            abilities.Add(new Ab_Enth_Enshadow());
            abilities.Add(new Ab_Soc_ShareEvidence());
            abilities.Add(new Ab_Soc_BoycottVote());
            abilities.Add(new Ab_Soc_DenounceOther());
            abilities.Add(new Ab_Soc_SwitchVote());
            abilities.Add(new Ab_UnholyFlesh_Attack());
            abilities.Add(new Ab_UnholyFlesh_Defend());
            abilities.Add(new Ab_UnholyFlesh_Grow());
            abilities.Add(new Ab_UnholyFlesh_Seed());
            abilities.Add(new Ab_Over_CancelVote());
        }

        public void turnTick()
        {
            hasTakenAction = false;
            power += map.param.overmind_powerRegen;
            power = Math.Min(power, map.param.overmind_maxPower);

            processEnthralled();
            int count = 0;
            double sum = 0;
            foreach (Location loc in map.locations)
            {
                if (loc.person() != null) { sum += loc.person().shadow;count += 1; }
            }
            map.data_avrgEnshadowment = sum / count;
            if (map.data_avrgEnshadowment > map.param.victory_targetEnshadowmentAvrg)
            {
                World.log("VICTORY DETECTED");
                map.world.prefabStore.popMsg("VICTORY ACHIEVED. Well done");
            }
        }

        public void processEnthralled()
        {
            if (enthralled == null) { return; }

            if (enthralled.isDead) { enthralled = null; }
        }
        public int countAvailableAbilities(Hex hex)
        {
            if (hex == null) { return 0; }
            if (hex.location == null) { return 0; }
            int n = 0;
            foreach (Ability a in abilities)
            {
                if (a.castable(map, hex))
                {
                    n += 1;
                }
            }
            return n;
        }
        public List<Ability> getAvailableAbilities(Hex hex)
        {
            if (hex == null) { return new List<Ability>(); }
            if (hex.location == null) { return new List<Ability>(); }
            List<Ability> reply = new List<Ability>();
            foreach (Ability a in abilities)
            {
                if (a.castable(map, hex))
                {
                    reply.Add(a);
                }
            }
            return reply;
        }
    }
}
