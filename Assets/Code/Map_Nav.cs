using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public partial class Map
    {

        public bool isSea(Location loc)
        {
            return isSea(loc.hex);
        }
        public bool isSea(Hex hex)
        {
            return !landmass[hex.x][hex.y];
        }
        public bool canGet(int x, int y)
        {
            return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
        }
        public double getDist(Location a, Location b)
        {
            return getDist(a.hex, b.hex);
        }
        public double getDist(Hex a, Hex b)
        {
            return Math.Sqrt(getSqrDist(a, b));
        }

        public int getStepDist(Location a, Location b)
        {
            if (a == b) { return 0; }

            //Expand in waves from the starting point, each time adding those on the border
            //We know the distance, since each border is 1 distance more
            HashSet<Location> seen = new HashSet<Location>();
            HashSet<Location> open = new HashSet<Location>();
            open.Add(a);
            seen.Add(a);
            int nSteps = 0;
            while (true)
            {
                HashSet<Location> border = new HashSet<Location>();
                nSteps += 1;
                foreach (Location loc in open)
                {
                    foreach (Location l2 in getNeighbours(loc))
                    {
                        if (seen.Contains(l2)) { continue; }
                        if (l2 == b) { return nSteps; }
                        border.Add(l2);
                        seen.Add(l2);
                    }
                }
                //The border is now used as the opens
                open.Clear();
                foreach (Location loc in border)
                {
                    open.Add(loc);
                }

                if (nSteps > 1024)
                {
                    throw new Exception("Map discontinuity detected");
                }
            }

            
        }

        public double getSqrDist(Hex a, Hex b)
        {
            if (a.y % 2 == b.y % 2)
            {
                return ((a.x - b.x) * (a.x - b.x)) + ((a.y - b.y) * (a.y - b.y));
            }
            float x1 = a.x;
            float y1 = a.y;
            float x2 = b.x;
            float y2 = b.y;
            if (a.y % 2 == 1)
            {
                x1 += 0.5f;
            }
            else
            {
                x2 += 0.5f;
            }
            return ((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2));
        }

        public List<int[]> getNeighbours(int x, int y)
        {
            List<int[]> reply = new List<int[]>();
            if (canGet(x + 1, y)) { reply.Add(new int[] { x + 1, y }); }
            if (canGet(x - 1, y)) { reply.Add(new int[] { x - 1, y }); }
            if (y % 2 == 0)
            {//Even, can add 1
                if (canGet(x + 1, y + 1)) { reply.Add(new int[] { x + 1, y + 1 }); }
                if (canGet(x, y + 1)) { reply.Add(new int[] { x, y + 1 }); }
                if (canGet(x + 1, y - 1)) { reply.Add(new int[] { x + 1, y - 1 }); }
                if (canGet(x, y - 1)) { reply.Add(new int[] { x, y - 1 }); }
            }
            else
            {//Odd, can subtract 1
                if (canGet(x - 1, y + 1)) { reply.Add(new int[] { x - 1, y + 1 }); }
                if (canGet(x, y + 1)) { reply.Add(new int[] { x, y + 1 }); }
                if (canGet(x - 1, y - 1)) { reply.Add(new int[] { x - 1, y - 1 }); }
                if (canGet(x, y - 1)) { reply.Add(new int[] { x, y - 1 }); }
            }

            return reply;
        }

        public int getRotation(Hex a, Hex b)
        {
            for (int i = 0; i < 6; i++)
            {
                if (getNeighbourRelative(a, i) == b) { return i; }
            }
            return -1;
        }

        public Hex getNeighbourRelative(Hex core, int rotation)
        {
            int i = rotation;
            bool right = false;
            int y = 0;
            if (i == 0) { right = false; y = 0; }
            else if (i == 1) { right = false; y = -1; }
            else if (i == 2) { right = true; y = -1; }
            else if (i == 3) { right = true; y = 0; }
            else if (i == 4) { right = true; y = 1; }
            else if (i == 5) { right = false; y = 1; }

            return getNeighbourRelative(core, y, right);
        }

        public Hex getNeighbourRelative(Hex core, int up, bool right)
        {
            if (up > 1) { up = 1; }
            if (up < -1) { up = -1; }
            int x = core.x; int y = core.y;
            if (up == 0)
            {
                if (!right)
                {
                    if (canGet(x - 1, y)) { return grid[x-1][ y]; } else { return null; }
                }
                else
                {
                    if (canGet(x + 1, y)) { return grid[x + 1][ y]; } else { return null; }
                }
            }
            else if (y % 2 == 0)
            {//Even, add one
                if (right)
                {
                    if (canGet(x + 1, y + up)) { return grid[x + 1][ y + up]; } else { return null; }
                }
                else
                {
                    if (canGet(x, y + up)) { return grid[x][ y + up]; } else { return null; }
                }
            }
            else
            {//Odd, remove one

                if (right)
                {
                    if (canGet(x, y + up)) { return grid[x][ y + up]; } else { return null; }
                }
                else
                {
                    if (canGet(x - 1, y + up)) { return grid[x-1][ y + up]; } else { return null; }
                }
            }
        }

        public List<Hex> getNeighbours(Hex hex)
        {
            List<Hex> reply = new List<Hex>();
            if (canGet(hex.x + 1, hex.y)) { reply.Add(grid[hex.x + 1][ hex.y]); }
            if (canGet(hex.x - 1, hex.y)) { reply.Add(grid[hex.x - 1][ hex.y]); }
            if (hex.y % 2 == 0)
            {//Even, can add 1
                if (canGet(hex.x + 1, hex.y + 1)) { reply.Add(grid[hex.x + 1][ hex.y + 1]); }
                if (canGet(hex.x, hex.y + 1)) { reply.Add(grid[hex.x][ hex.y + 1]); }
                if (canGet(hex.x + 1, hex.y - 1)) { reply.Add(grid[hex.x + 1][ hex.y - 1]); }
                if (canGet(hex.x, hex.y - 1)) { reply.Add(grid[hex.x][ hex.y - 1]); }
            }
            else
            {//Odd, can subtract 1
                if (canGet(hex.x - 1, hex.y + 1)) { reply.Add(grid[hex.x - 1][ hex.y + 1]); }
                if (canGet(hex.x, hex.y + 1)) { reply.Add(grid[hex.x][ hex.y + 1]); }
                if (canGet(hex.x - 1, hex.y - 1)) { reply.Add(grid[hex.x - 1][ hex.y - 1]); }
                if (canGet(hex.x, hex.y - 1)) { reply.Add(grid[hex.x][ hex.y - 1]); }
            }

            return reply;
        }

        public List<Location> getNeighbours(Location loc)
        {
            List<Location> reply = new List<Location>();
            foreach (Link l in loc.links)
            {
                reply.Add(l.other(loc));
            }
            return reply;
        }
    }
}
