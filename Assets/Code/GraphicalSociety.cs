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

                    s.subtitle.text = "";
                    s.lowerRightText.text = "";
                }
                else
                {
                    RelObj rel = focus.relations[s.inner];
                    float liking = (float)rel.getLiking() / 100;
                    if (i == 0)
                        Debug.Log(rel.suspicion.ToString());

                    float radius = 3.5f - liking * 0.75f;
                    float x = Mathf.Cos(6.28f / n * i) * radius;
                    float y = Mathf.Sin(6.28f / n * i) * radius;

                    s.targetPosition = new Vector3(x, y, 0.0f);
                    if (liking < 0)
                        s.targetColor = Color.Lerp(s.neutralColor, s.badColor, -liking);
                    else
                        s.targetColor = Color.Lerp(s.neutralColor, s.goodColor, liking);
                    s.targetColor.a = 0.5f;
                    //s.targetColor.a = 0.1f + ((float)rel.suspicion / 100);

                    s.subtitle.text = "Relationship with " + focus.firstName;
                    s.lowerRightText.text  = "Liked by: " + focus.relations[s.inner].getLiking().ToString("N2");
                    s.lowerRightText.text += "\nLikes: " + s.inner.relations[focus].getLiking().ToString("N2");

                    i += 1;
                }
            }
        }

        public static void showHover(Person p)
        {
            //
        }

        public static void purge()
        {
            loadedSlots.Clear();
        }
    }
}
