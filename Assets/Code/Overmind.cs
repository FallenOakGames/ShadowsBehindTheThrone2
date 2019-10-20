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

        public Overmind(Map map)
        {
            this.map = map;
            abilities.Add(new Ab_TestAddShadow());
            abilities.Add(new Ab_UnholyFlesh_Attack());
            abilities.Add(new Ab_UnholyFlesh_Defend());
            abilities.Add(new Ab_UnholyFlesh_Grow());
            abilities.Add(new Ab_UnholyFlesh_Seed());
        }

        public void turnTick()
        {
            hasTakenAction = false;
            power += map.param.overmind_powerRegen;
            power = Math.Min(power, map.param.overmind_maxPower);
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
            if (hex == null) { return null; }
            if (hex.location == null) { return null; }
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
