using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class LogBox
    {
        public string path;
        public LogBox(Person p)
        {
            path = "logging" + World.separator + "people" + World.separator + p.firstName + ".log";
            System.IO.File.WriteAllLines(path,new string[]{ "Log for " + p.firstName});
        }

        public void takeLine(string line)
        {

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(path, true))
            {
                file.WriteLine(line);
            }
        }

    }
}