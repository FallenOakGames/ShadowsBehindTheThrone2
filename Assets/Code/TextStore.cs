using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Assets.Code
{
    public class TextStore
    {
        public static SerialMap<string, List<string>> flavourLists = new SerialMap<string, List<string>>();
        //public static string[] firstNamesM;
        //public static string[] firstNamesF;
        //public static string[] lastNames;

        public static MarkovModel model;
        public static MarkovModel amerimodel;
        public static MarkovModel frenchModel;
        public static HashSet<string> usedCityNames = new HashSet<string>();
        public static HashSet<string> usedLocNames = new HashSet<string>();
        
        public static HashSet<string> verboten = new HashSet<string>();

        public static void buildVerboten()
        {
            verboten.Add("dong");
            verboten.Add("wang");
            verboten.Add("mong");
            verboten.Add("dick");
            verboten.Add("fuck");
            verboten.Add("cunt");
            verboten.Add("shit");
            verboten.Add("crap");

            MarkovModel capper = new MarkovModel();
            HashSet<string> lowerV = new HashSet<string>();
            foreach (string s in verboten)
            {
                lowerV.Add(s);
            }
            foreach (string s in lowerV)
            {
                verboten.Add(capper.capFirst(s));
            }
        }

        public static string getFlavour(string key)
        {
            if (flavourLists.ContainsKey(key))
            {
                List<string> opts = flavourLists.get(key);
                int q = Eleven.random.Next(opts.Count);
                return opts[q];
            }
            else
            {
                return key;
            }
        }

        public static string getName(bool male)
        {
            for (int i = 0; i < 100; i++)
            {
                string word = amerimodel.capFirst(amerimodel.getWord());
                if (verboten.Contains(word)) { continue; }
                return word;
            }
            return "Alhazred";
        }

        public static string getCityName()
        {
            for (int t = 0; t < 100; t++)
            {
                string reply = model.capFirst(model.getWord());
                if (verboten.Contains(reply))
                {
                    //World.Log("Caught verboten word: " + reply);
                    continue;
                }
                if (usedCityNames.Contains(reply))
                {
                    continue;
                }
                usedCityNames.Add(reply);
                return reply;
            }
            return model.capFirst(model.getWord());
        }

        public static string getLocName()
        {
            for (int t = 0; t < 100; t++)
            {
                string reply = model.capFirst(frenchModel.getWord());
                if (verboten.Contains(reply))
                {
                    //World.Log("Caught verboten word: " + reply);
                    continue;
                }
                if (usedLocNames.Contains(reply))
                {
                    continue;
                }
                usedLocNames.Add(reply);
                return reply;
            }
            return model.capFirst(model.getWord());
        }
        public void loadLinux(World world)
        {

            loadFlavourLinux(world);
            string[] markovFood = System.IO.File.ReadAllLines(world.pathPrefix + "/data/words/chineseCities.txt");
            string[] markovFood2 = System.IO.File.ReadAllLines(world.pathPrefix + "/data/words/frenchCities.txt");
            string[] markovFood3 = System.IO.File.ReadAllLines(world.pathPrefix + "/data/words/french.txt");

            buildVerboten();
            model = new MarkovModel();
            model.buildModel(markovFood);
            amerimodel = new MarkovModel();
            amerimodel.buildModel(markovFood2);
            frenchModel = new MarkovModel();
            frenchModel.buildModel(markovFood3);

            string key;
            string value;
            key = "VICTORY";
            value = "Another day ends, another sun sets.";
            key = "VICTORY";
            value = "\"The heavens are mourning the death of the sun.\"";
            put(key, value);
            key = "DEFEAT";
            value = "The sun shines brighter, the night seems shorter.";
            put(key, value);
        }

        public void load()
        {
            loadFlavour();
            string[] markovFood = System.IO.File.ReadAllLines(".\\data\\words\\chineseCities.txt");
            string[] markovFood2 = System.IO.File.ReadAllLines(".\\data\\words\\frenchCities.txt");
            string[] markovFood3 = System.IO.File.ReadAllLines(".\\data\\words\\french.txt");

            buildVerboten();
            model = new MarkovModel();
            model.buildModel(markovFood);
            amerimodel = new MarkovModel();
            amerimodel.buildModel(markovFood2);
            frenchModel = new MarkovModel();
            frenchModel.buildModel(markovFood3);


            string key;
            string value;

            key = "MONARCHISM";
            value = "Political view: Monarchism. \nMonarchism generally views societal order as deriving from a single inherited "
                + "position of power, passed down the generations. Subordinate ranks are similarly inherited, but people retain "
                + "the position they were born into.";
            put(key, value);
            key = "MONARCHISM";
            value = "Good rule flows from royal blood. Monarchism spreads.";
            put(key, value);
            key = "REPUBLICANISM";
            value = "Political view: Republicanism. \nRepublicanism opposes monarchism, and argues towards an equality between all citizens."
                + " While in this day and age this equality may not be considered to extend to the low born, some of the nobility consider it "
                + " reasonable that a bad king might be replaced by a good duke.";
            put(key, value);
            key = "REPUBLICANISM";
            value = "The very beginnings of freedom stir, with the thought that even a baroness may one day rise to be queen.";
            put(key, value);
            key = "MILITARISM";
            value = "Militarism increases. Fundamentally, all diplomacy is simply threats of force or promises of force dressed up sufficiently to be palatable to the masses.";
            put(key, value);
            key = "PACIFISM";
            value = "Militarism recedes. Surely there must be more to life than constant warfare.";
            put(key, value);
            key = "ISOLATIONISM";
            value = "Isolationism, xenophobia, closed doors and locked gates. Bristled spears pointed at all travellers "
                + "and roads blocked to traders. The only sure defence against epidemic plague.";
            put(key, value);
            key = "MULTICULTURALISM";
            value = "Isolationism cannot compete on the market. The further the trader has come, the more his wares will be worth.";
            put(key, value);



            key = "FISHMANRAID";
            value = "\"Without warning an entire army marched up the beach, staggering with legs unaccustomed to land, but well armed and as strong as a bull. They listened to no plea, no negotiation, no threat. We were not ready, and, I fear, will not be ready the next time either.\"";
            put(key, value);
            key = "FISHMANRAID";
            value = "\"One presumes to them the convulsive gurgling must have been laughter, as they dragged their victims and spoils back beneath the waves. AS they gathered their prizes their mouths opened wider than any "
                + " human mouth could, with rows and rows of needle-like teeth, gibbering that awful sound.\"";
            put(key, value);

            key = "DEATH";
            value = "The last grain of sand falls and the hourglass sits silent. Not necessarily gone, just waiting for"
                + " someone or something to upend it, and allow it to flow again, albeit this time in reverse.";
            put(key, value);
            key = "VICTORY";
            value = "Another day ends, another sun sets.";
            key = "VICTORY";
            value = "\"The heavens are mourning the death of the sun.\"";
            put(key, value);
            key = "DEFEAT";
            value = "The sun shines brighter, the night seems shorter.";
            put(key, value);
            key = "CITYDESTRUCTION";
            value = "A metropole which one swarmed and churned with life falls silent and empty.";
            put(key, value);
            key = "SOCIETYGONE";
            value = "Another flag falls to the ground.";
            put(key, value);
            key = "PEASANTREBELS";
            value = "Politics? For the proles? What happened to panem et circenses?";
            put(key, value);
            key = "PEASANTREBELS";
            value = "Eventually the pawns will tire of dying for the sake of another's advancement, will question why their lives are currency for another to spend. We hope that day is not soon.";
            put(key, value);

            key = "LETTER_ACCUSE";
            value = "You are an evil thing. Your name is forever damned.";
            put(key, value);
        }

        public void loadFlavour()
        {
            string[] filePaths = Directory.GetFiles(".\\data\\words\\flavour");
            foreach (string fileName in filePaths)
            {

                if (fileName.EndsWith(".txt"))
                {
                    //World.log("Flavour seen: " + fileName);
                    string[] words = System.IO.File.ReadAllLines(fileName);
                    string title = words[0];
                    string body = "";
                    for (int i = 1; i < words.Length; i++)
                    {
                        body += words[i] + "\n";
                    }
                    //World.log("Flavour read. Title: " + title + " body: " + body);
                    put(title, body);
                }
            }
        }
        public void loadFlavourLinux(World world)
        {
            string[] filePaths = Directory.GetFiles(world.pathPrefix + "/data/words/flavour");
            foreach (string fileName in filePaths)
            {

                if (fileName.EndsWith(".txt"))
                {
                    //World.log("Flavour seen: " + fileName);
                    string[] words = System.IO.File.ReadAllLines(fileName);
                    string title = words[0];
                    string body = "";
                    for (int i = 1; i < words.Length; i++)
                    {
                        body += words[i] + "\n";
                    }
                    //World.log("Flavour read. Title: " + title + " body: " + body);
                    put(title, body);
                }
            }
        }

        public void put(string key, string val)
        {
            List<string> existing = new List<string>();
            if (flavourLists.ContainsKey(key))
            {
                existing = flavourLists.get(key);
            }
            else
            {
                flavourLists.put(key, existing);
            }
            existing.Add(val);
        }

        public string lookup(string key)
        {
            if (flavourLists.ContainsKey(key))
            {
                List<string> opts = flavourLists.get(key);
                int q = Eleven.random.Next(opts.Count);
                return opts[q];
            }
            return key;
        }

        public bool hasLookupKey(string key)
        {
            return flavourLists.ContainsKey(key);
        }
    }
}
