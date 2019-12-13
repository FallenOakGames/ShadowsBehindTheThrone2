using OdinSerializer;
﻿using UnityEngine;
using UnityEditor;

namespace Assets.Code
{
    public class LogBox : SerializedScriptableObject
    {
        System.IO.StreamWriter file;
        public string path;

        public LogBox(string fileName)
        {
            path = fileName;
        }

        public void takeLine(string line)
        {

            //Don't use this as a field, or it'll get maximally shrivelled when serialising
            //Just don't log so much you faithless wretch
            
            //if (file == null)
            //{
            //     file = new System.IO.StreamWriter(path, true);
            //}
            //file.WriteLine(line);

            //file.Close();
            
        }

    }
}