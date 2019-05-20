using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public partial class Map
    {
        public List<Location> majorLocations = new List<Location>();
        public List<Location> locations = new List<Location>();
        public Hex[][] grid;//MUST be a jagged implement. For serialisation
        public World world;
        public bool burnInComplete = false;
        public MapMaskManager masker;
        public Globalist globalist = new Globalist();
        public int turn;
        public List<SocialGroup> socialGroups = new List<SocialGroup>();
        public HashSet<string> permaDismissed = new HashSet<string>();
        //public MapEventManager eventManager;
        //public StatRecorder stats;
        public float lastTurnTime;
        public Params param;

        public Map(Params param)
        {
            this.param = param;
            masker = new MapMaskManager(this);
            //overmind = new Overmind(this);
            //eventManager = new MapEventManager(this);
           // stats = new StatRecorder(this);
        }

        public void turnTick()
        {
            turn += 1;

            lastTurnTime = UnityEngine.Time.fixedTime;
            //eventManager.turnTick();
            //overmind.turnTick();
            //panic.turnTick();


            //Then grid cells
            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    grid[i][j].turnTick();
                }
            }

            //Wars
            processWars();

            //Finally societies
            //Use a duplication list so they can modify the primary society list (primarly adding a child soc)
            List<SocialGroup> duplicate = new List<SocialGroup>();
            foreach (SocialGroup group in socialGroups) { duplicate.Add(group); }
            foreach (SocialGroup group in duplicate)
            {
                group.turnTick();
            }

            List<SocialGroup> rems = new List<SocialGroup>();
            foreach (SocialGroup group in socialGroups)
            {
                if (group.isGone()) { rems.Add(group); }
            }
            foreach (SocialGroup g in rems)
            {
                socialGroups.Remove(g);
            }
            
            //stats.turnTick();
        }

        public void processWars()
        {
            //Every society decides which other to attack, assuming it is over threshold combat strength
            foreach (SocialGroup sg in socialGroups)
            {
                if (sg.lastBattle == turn) { continue; }//Only one battle permitted per social group (that they initiate, at least)

                if (sg.currentMilitary < sg.maxMilitary * param.combat_thresholdAttackStrength) { continue; }//Below min strength

                int c = 0;
                Location attackFrom = null;
                Location attackTo = null;
                foreach (Location l in locations)
                {
                    if (l.soc == sg)
                    {
                        foreach (Link link in l.links)
                        {
                            if (link.other(l).soc != null && link.other(l).soc != sg && link.other(l).soc.getRel(sg).state == DipRel.dipState.war)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    attackFrom = l;
                                    attackTo = link.other(l);
                                }
                            }
                        }
                    }
                }
                if (attackFrom != null)
                {
                    SocialGroup defender = attackTo.soc;

                    World.log(sg.getName() + " attacking into " + attackTo.getName());
                    double myStr = sg.currentMilitary * Eleven.random.NextDouble();
                    double theirStr =  defender.currentMilitary * Eleven.random.NextDouble();

                    //Note the defensive fortifications only reduce losses, not increase chance of taking territory
                    double myLosses = Math.Min(sg.currentMilitary, theirStr * param.combat_lethality);
                    sg.currentMilitary -= myLosses;
                    double theirLosses = (1 - attackTo.getMilitaryDefence())*Math.Min(defender.currentMilitary, myStr * param.combat_lethality);
                    defender.currentMilitary -= theirLosses;

                    if (myStr > theirStr * param.combat_takeLandThreshold)
                    {
                        takeLocationFromOther(sg, defender,attackTo);
                    }
                    else if (theirStr > myStr * param.combat_takeLandThreshold)
                    {
                        takeLocationFromOther(defender,sg,attackFrom);
                    }
                }
            }

            foreach (SocialGroup group in socialGroups)
            {
                foreach (DipRel rel in group.getAllRelations())
                {
                    if (rel.state == DipRel.dipState.war && rel.war.canTimeOut)
                    {
                        if (turn - rel.war.startTurn > param.war_defaultLength)
                        {
                            declarePeace(rel);
                        }
                    }
                }
            }
        }

        public void takeLocationFromOther(SocialGroup att,SocialGroup def,Location taken)
        {
            World.log(att.getName() + " takes " + taken.getName() + " from " + def.getName());

            if (taken.settlement != null)
            {
                if (taken.settlement.title != null && taken.settlement.title.heldBy != null)
                {
                    Person lord = taken.settlement.title.heldBy;
                    if (def is Society)
                    {
                        Society socDef = (Society)def;
                        if (socDef.people.Contains(lord) == false) { throw new Exception("Lord wasn't in the society holding their land"); }
                        socDef.people.Remove(lord);
                    }
                    if (att is Society)
                    {
                        Society socAtt = (Society)att;
                        if (socAtt.people.Contains(lord)) { throw new Exception("Lord already in invading social group's people"); }
                        socAtt.people.Add(lord);
                        World.log(lord.getFullName() + " now under the rule of " + att.getName());
                    }
                    else
                    {
                        lord.die("Killed by " + att.getName() + " when " + taken.getName() + " fell");
                    }
                }
            }

            taken.soc = att;
        }

        private void declarePeace(DipRel rel)
        {
            World.log("Peace breaks out between " + rel.a.getName() + " and " + rel.b.getName());

            rel.war = null;
            rel.state = DipRel.dipState.none;
        } 

        public void declareWar(SocialGroup att,SocialGroup def)
        {
            World.log(att.getName() + " declares war on " + def.getName());

            att.getRel(def).state = DipRel.dipState.war;
            att.getRel(def).war = new War(this,att, def);
        } 
    }
}
