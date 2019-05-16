using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Person
    {
        public string firstName;
        public bool male = Eleven.random.Next(2) == 0;
        public Title_Land title_land;
        public Society society;
        public Dictionary<Person, RelObj> relations = new Dictionary<Person, RelObj>();
        public double prestige = 1;

        public Person(Society soc)
        {
            this.society = soc;
            firstName = TextStore.getName(male);
            getRelation(this).value = 100;//Set self-relation to 100
        }

        public Map map { get { return society.map; } }

        public string getFullName()
        {
            return getTitles() + " " + firstName; 
        }

        public RelObj getRelation(Person other)
        {
            if (relations.ContainsKey(other))
            {
                return relations[other];
            }
            RelObj rel = new RelObj(this, other,map.param.relObj_defaultLiking);
            relations.Add(other, rel);
            return rel;
        }

        public string getTitles()
        {
            if (male)
            {
                if (title_land != null) { return title_land.titleM; }
                return "Lord";
            }else
            {
                if (title_land != null) { return title_land.titleF; }
                return "Lady";
            }
        }
    }
}
