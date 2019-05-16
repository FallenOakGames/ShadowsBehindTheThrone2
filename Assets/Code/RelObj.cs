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
        public double value;
        private Person other;
        private Person person;

        public RelObj(Person person, Person other,double initialVal)
        {
            this.person = person;
            this.other = other;
            value = initialVal;
        }
    }
}
