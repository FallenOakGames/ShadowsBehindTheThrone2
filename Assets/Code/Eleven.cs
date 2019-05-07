using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class Eleven
    {
        public static Random random = new Random();


        public static string putCommas(double d)
        {
            return putCommas((int)d);
        }
        public static string putCommas(int d)
        {
            string s = "" + (int)d;
            char[] array = s.ToCharArray();

            string reply = "";
            for (int i = 0; i < s.Length; i++)
            {
                int remain = (i - s.Length) + 1;
                reply += array[i];
                if (remain != 0 && remain % 3 == 0)
                {
                    reply += ",";
                }
            }

            return reply;
        }
    }
}
