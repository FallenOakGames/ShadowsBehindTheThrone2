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
        private Person other;
        private Person person;

        public RelObj(Person person, Person other,double initialVal)
        {
            this.person = person;
            this.other = other;
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
            RelObj rel = this;
            double baseline = person.getRelBaseline(rel.them);
            rel.liking -= baseline;
            rel.liking *= person.map.param.relObj_decayRate;
            rel.liking += baseline;
            if (rel.liking > 100) { rel.liking = 100; }
            if (rel.liking < -100) { rel.liking = -100; }
        }

        public void addLiking(double v)
        {
            liking += v;
        }
    }
}
