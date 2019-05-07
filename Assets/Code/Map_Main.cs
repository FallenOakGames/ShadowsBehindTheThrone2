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
        public Params para = new Params();
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
    }
}
