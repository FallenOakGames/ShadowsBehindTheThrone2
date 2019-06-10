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
                /*
                if (command == "evidence")
                {
                    GraphicalMap.selectedHex.location.evidence.Add(new Evidence(map));
                }
                else if (command == "world ev" || command == "world evidence")
                {
                    map.overmind.worldEvidence += map.overmind.evidenceThreshold;
                }
                else if (command == "panic")
                {
                    map.panic.worldPanic = 1;
                }
                else if (command == "no popups")
                {
                    map.world.displayMessages = !map.world.displayMessages;
                    World.log("Display messages: " + map.world.displayMessages);
                }
                else if (command == "lightbringer")
                {
                    GraphicalMap.selectedHex.location.settlement.lordSlot.person.state = Person.darkState.lightbringer;
                }
                else if (command == "enshadow")
                {
                    GraphicalMap.selectedHex.location.settlement.lordSlot.person.shadow = 1;
                }
                else if (command == "start plague")
                {
                    GraphicalMap.selectedHex.location.settlement.population.diseases[map.globalist.disease_plague.index] = 0.05;
                }
                else if (command == "glasskin")
                {
                    Unit_GlassSkinProphet prophet = new Unit_GlassSkinProphet(map, null, GraphicalMap.selectedHex.location, false);
                    map.allUnits.Add(prophet);
                    prophet.loc.units.Add(prophet);
                }
                else if (command == "spawn ghoul")
                {
                    Unit_Ghouls ghoul = new Unit_Ghouls(map, map.soc_undead, GraphicalMap.selectedHex.location, map.soc_undead.ai, true);
                    ghoul.hp = 100;
                    ghoul.loc = GraphicalMap.selectedHex.location;
                    ghoul.loc.units.Add(ghoul);
                    map.allUnits.Add(ghoul);
                }
                else if (command == "power")
                {
                    map.overmind.power = 10000;
                }
                else if (command == "cash")
                {
                    map.overmind.cash = 10000;
                }
                else if (command == "make guilty")
                {
                    foreach (SocialGroup g in map.socialGroups)
                    {
                        g.killOnSight.Add(GraphicalMap.selectedUnit);
                    }
                }
                else if (command == "zeit")
                {
                    Society soc = (Society)GraphicalMap.selectedHex.location.soc;
                    soc.takeZeit(Zeit.zeitEvidenceUncovered(map));
                }
                else if (command == "civil war")
                {
                    Society soc = (Society)GraphicalMap.selectedHex.location.soc;
                    for (int i = 0; i < soc.command.top.subs.Count; i++)
                    {
                        if (soc.command.top.subs[i].person != null)
                        {
                            soc.command.top.subs[i].person.getRel(soc.command.top.person).addLiking(-50);
                        }
                    }
                }
                */
            }
            catch(Exception e)
            {
                World.log(e.Message);
            }
        }
    }
}
