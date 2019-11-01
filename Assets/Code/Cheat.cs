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
                if (command == "vote")
                {
                    foreach (Location l in map.locations)
                    {
                        if (l.person() != null && l.person().state == Person.personState.enthralled)
                        {
                            World.log("Found enthralled");
                            Society soc = (Society)l.soc;
                            if (soc.voteSession != null) {
                                World.log("Attempting to build blocker");
                                map.world.ui.addBlocker(map.world.prefabStore.getScrollSet(soc.voteSession, soc.voteSession.issue.options).gameObject);
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                World.log(e.Message);
            }
        }
    }
}
