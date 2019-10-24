using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

namespace Assets.Code
{
    public class GraphicalSociety
    {
        public static World world;

        public static Society activeSociety;
        public static Person focus;
        public static List<GraphicalSlot> loadedSlots = new List<GraphicalSlot>();

        public static void setup(Society soc)
        {
            activeSociety = soc;
            foreach (Person p in activeSociety.people)
            {
                GraphicalSlot slot = world.prefabStore.getGraphicalSlot(p);
            }

            if (loadedSlots.Count != 0)
                refresh(activeSociety.people[0]);
        }

        public static void refresh(Person pf)
        {
            focus = pf;

            int n = activeSociety.people.Count - 1, i = 0;
            foreach (GraphicalSlot s in loadedSlots)
            {
                if (s.inner == focus)
                {
                    s.targetPosition = Vector3.zero;
                }
                else
                {
                    RelObj rel = focus.relations[s.inner];

                    float radius = 3 + ((float)rel.getLiking() / 50);
                    float x = Mathf.Cos(6.28f / n * i) * radius;
                    float y = Mathf.Sin(6.28f / n * i) * radius;

                    s.targetPosition = new Vector3(x, y, 0);
                    i += 1;
                }
            }
        }

        public static void purge()
        {
            loadedSlots.Clear();
        }
    }
}
