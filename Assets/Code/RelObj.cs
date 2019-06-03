using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class RelObj
    {
        public Person me;
        public Person them;
        //public double liking { get { return liking; } set { liking = value;liking = Math.Min(100, liking);liking = Math.Max(-100, liking); } } THIS IMPLEMENTATION CRASHES THE UNITY EDITOR
        private double liking;

        public RelObj(Person person, Person other,double initialVal)
        {
            this.me = person;
            this.them = other;
            liking = initialVal;
        }
        public double getLiking()
        {
            return liking;
        }
        
        public void setLiking(double v)
        {
            liking += v;
            if (liking > 100) { liking = 100; }
            if (liking < -100) { liking = -100; }
        }
        public void turnTick()
        {
            if (them == me) { liking = 100; }//Be at least loyal to yourself (till traits override this)

            double baseline = me.getRelBaseline(them);
            liking -= baseline;
            liking *= me.map.param.relObj_decayRate;
            liking += baseline;
            if (liking > 100) { liking = 100; }
            if (liking < -100) { liking = -100; }
        }

        public void addLiking(double v)
        {
            liking += v;
        }
    }
}
