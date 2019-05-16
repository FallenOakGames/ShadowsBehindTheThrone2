using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteMsg
    {
        public double value;
        public string msg;

        public VoteMsg(string v, double u)
        {
            this.msg = v;
            this.value = u;
        }
    }
}
