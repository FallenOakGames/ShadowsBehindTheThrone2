using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Cheat
    {
        public static bool controlAll = false;

        public static void takeCommand(Map map,string command)
        {
            World.log("cheat command registered: " + command);


            try
            {
                if (command == "power")
                {
                    map.overmind.power = 100;
                }
                if (command == "shadow")
                {
                    GraphicalMap.selectedHex.location.person().shadow = 1;
                }
                if (command == "love")
                {
                    foreach (Person p in map.overmind.enthralled.society.people)
                    {
                        p.getRelation(map.overmind.enthralled).addLiking(100, "Cheat love", map.turn);
                    }
                }
                if (command == "evidence")
                {
                    GraphicalMap.selectedHex.location.person().evidence = 1;
                }
            }
            catch(Exception e)
            {
                World.log(e.Message);
            }
        }
    }
}
