using OdinSerializer;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.UI;
using UnityEngine;
using FullSerializer;

namespace Assets.Code
{
    /*
     * World is your monobehaviour master. It calls the start function, triggering all loading and suchlikes and suchforths
     *
     * It has the ONLY reference to map. Map must be kept apart from any Unity gameObjects, so it can be serialised out
     * Obviously this is impossible, but every class which knows about its unity GameObject must be able to purge this reference : SerializedScriptableObject
     *
     * It holds the references to the 'stores'. These are repositories which should not be serialised into the saved games
     */
    public class World : MonoBehaviour
    {
        public static bool logging   = false;
        public static bool developer = false;

        public UIMaster ui;
        public TextureStore textureStore;
        public PrefabStore prefabStore;
        public Camera outerCamera;
        public TextStore wordStore;
        //public SoundObj soundSource;

        public Map map;
        public static Map staticMap;
        public bool displayMessages = false;
        public bool turnLock = false;
        public string pathPrefix = "";
        public static string separator = "";
        public bool isWindows = false;

        public static LogBox saveLog;

        public void Start()
        {
            saveLog = new LogBox("saveLog.log");
            Screen.SetResolution(1920, 1080, true);
            if (developer)
                startup();
            else
                ui.setToMainMenu();

            //ui.setToMainMenu();
            //startup();
            //ui.setToWorld();
        }

        public float lastFrame;
        public void Update()
        {
            if (lastFrame == -1) {
                lastFrame = Time.realtimeSinceStartup;
                return;
            }
            else
            {
                float dT = Time.realtimeSinceStartup - lastFrame;
                float targetDT = 1 / 60;
                if (dT < targetDT)
                {
                    new WaitForSecondsRealtime(targetDT-dT);
                }
            }
        }

        public void specificStartup()
        {
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
            {
                Log("Windows! A windows OS");
                isWindows = true;
                pathPrefix = "";
                separator = "/";
                string[] decomp = Application.dataPath.Split('/');
                for (int i = 0; i < decomp.Length - 1; i++)
                {
                    pathPrefix += decomp[i] + "/";
                }

                textureStore.world = this;
                textureStore.load();
                wordStore = new TextStore();
                wordStore.load();
            }
            else //if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Linux)
            {
                Log("The operating system is Linux based");
                pathPrefix = "";
                separator = "/";
                string[] decomp = Application.dataPath.Split('/');
                for (int i = 0; i < decomp.Length - 1; i++)
                {
                    pathPrefix += decomp[i] + "/";
                }
                textureStore.world = this;
                textureStore.loadLinux();
                wordStore = new TextStore();
                wordStore.loadLinux(this);
            }

            //wordStore = new TextStore();
            //wordStore.load();
            Application.targetFrameRate = 60;
            GraphicalMap.world = this;
            //Activity.load();

            GraphicalSociety.world = this;

            if (logging)
            {
                foreach (string f in Directory.GetFiles("logging" + separator + "people"))
                {
                    if (f.Contains(".log"))
                    {
                        File.Delete(f);
                    }
                }
                foreach (string f in Directory.GetFiles("logging" + separator + "societies"))
                {
                    if (f.Contains(".log"))
                    {
                        File.Delete(f);
                    }
                }
            }
        }
        public void startup()
        {
            Log("Called startup");
            specificStartup();
            Params param = new Params();
            param.loadFromFile();
            map = new Map(param);
            GraphicalMap.map = map;
            GraphicalMap.world = this;

            Property_Prototype.loadProperties(map);
            EconTrait.loadTraits(map);
            map.world = this;
            map.gen();

            staticMap = map;
            if (!developer)
            {
                for (int i = 0; i < param.mapGen_burnInSteps; i++)
                {
                    map.turnTick();
                }
            }

            ui.setToWorld();
            displayMessages = true;
            Log("Got to end of initial startup");
            ui.checkData();
        }

        public void bQuit()
        {
            Application.Quit();
        }
        public void bViewPlayback()
        {
            ui.addBlocker(prefabStore.getPlayback(this, map).gameObject);
        }
        public void bStartGame()
        {
            startup();
        }

        public void bContinue()
        {
            ui.setToWorld();
        }

        public void bStartSeedlessGame()
        {
            Eleven.random = new System.Random(42069);
            startup();
        }

        public void bEndTurn()
        {
            if (turnLock) { return; }
            if (ui.blocker != null) { return; }

            turnLock = true;
            if (map != null) {
                map.turnTick();
            }

            turnLock = false;
            ui.checkData();
        }
        public void b10Turns()
        {
            if (turnLock) { return; }
            if (ui.blocker != null) { return; }

            turnLock = true;
            if (map != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    map.turnTick();
                }
            }

            turnLock = false;
            ui.checkData();
        }

        public void b100Turns()
        {
            if (turnLock) { return; }
            if (ui.blocker != null) { return; }

            turnLock = true;
            if (map != null)
            {
                for (int i = 0; i < 100; i++)
                {
                    map.turnTick();
                }
            }

            turnLock = false;
            ui.checkData();
        }
        public static void Log(string str)
        {
            Debug.Log(str);
        }
        public static void log(string str)
        {
            if (staticMap != null)
            {
                Debug.Log("Turn: " + staticMap.turn + ": " + str);
            }
            else
            {
                Debug.Log(str);
            }
        }


        public void testXScroll()
        {

            List<ThreatItem> threats = new List<ThreatItem>();
            for (int i = 0; i < 5; i++)
            {
                Person p = new Person((Society)map.socialGroups[0]);
                ThreatItem t = new ThreatItem(map, p);
                threats.Add(t);
            }
            ui.addBlocker(prefabStore.getScrollSetThreats(threats).gameObject);
        }

        public void testYScroll()
        {

            List<ThreatItem> threats = new List<ThreatItem>();
            for (int i = 0; i < 5; i++)
            {
                Person p = new Person((Society)map.socialGroups[0]);
                ThreatItem t = new ThreatItem(map, p);
                threats.Add(t);
            }
            ui.addBlocker(prefabStore.getScrollSetThreats(threats).gameObject);
        }

        public void save(string filename)
        {
            World world = this;
            world.ui.setToMainMenu();
            GraphicalMap.purge();
            GraphicalSociety.purge();
            world.map.world = null;

<<<<<<< Updated upstream

            /*
             * Okay, we're hard capped on save size
             * So we gotta split shit up into different blocks
             * 
             * -: hexes and maps
             * 
             * -: RelObjs
             * 
             * -: Playback
             * 
             * -: all remaining
             * 
             */
            SAVE_MapsAndHexes mapsAndHexes = new SAVE_MapsAndHexes();
            mapsAndHexes.grid = map.grid;
            mapsAndHexes.humidity = map.humidityMap;
            mapsAndHexes.temp = map.tempMap;
            map.grid = null;
            map.humidityMap = null;
            map.tempMap = null;
            StatRecorder recorder = map.stats;
            map.stats = null;

            subsave(filename, mapsAndHexes);
            subsave(filename, recorder);
            subsave(filename, map);



            world.map.world = world;
            staticMap = map;
            
            world.prefabStore.popMsg("Game saved as: " + filename);

            //// step 1: parse the JSON data
            //fsData data = fsJsonParser.Parse(serializedState);

            //// step 2: deserialize the data
            //object deserialized = null;
            //_serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
        }

        private void subsave(string filename, SAVE_MapsAndHexes mapsAndHexes)
        {
            fsSerializer _serializer = new fsSerializer();
            fsData data;
            _serializer.TrySerialize(typeof(SAVE_MapsAndHexes), mapsAndHexes, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            //string saveString = fsJsonPrinter.CompressedJson(data);
            string saveString = "Replaced";
            World.Log("Save exit point");

            filename = filename + "_mapsAndHexes.sv";
            if (File.Exists(filename))
            {
                World.Log("Overwriting old save: " + filename);
                File.Delete(filename);
            }
            File.WriteAllLines(filename, new string[] { saveString });
        }
        private void subsave(string filename, StatRecorder recorder)
        {

            world.prefabStore.popMsg("Game saved as: " + filename);

            fsSerializer _serializer = new fsSerializer();
            fsData data;
            _serializer.TrySerialize(typeof(StatRecorder), recorder, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            //string saveString = fsJsonPrinter.CompressedJson(data);
            string saveString = "Replaced";
            World.Log("Save exit point");

            filename = filename + "_recorder.sv";
            if (File.Exists(filename))
            {
                World.Log("Overwriting old save: " + filename);
                File.Delete(filename);
            }
            File.WriteAllLines(filename, new string[] { saveString });
        }
        private void subsave(string filename, Map map)
        {

            fsSerializer _serializer = new fsSerializer();
            fsData data;
            _serializer.TrySerialize(typeof(Map), map, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            //string saveString = fsJsonPrinter.CompressedJson(data);
            string saveString = "Replaced";
            World.Log("Save exit point");

            filename = filename + "_map.sv";
            if (File.Exists(filename))
            {
                World.Log("Overwriting old save: " + filename);
                File.Delete(filename);
            }
            File.WriteAllLines(filename, new string[] { saveString });
        }

        public void bQuicksave()
        {
            save("quicksave.sv");
        }
        public void bQuickload()
        {
            load("quicksave.sv");
        }
        public void load(string filename)
        {
            //try
            //{
                if (map != null)
                {
                    GraphicalMap.purge();
                    GraphicalSociety.purge();
                    map.world = null;
                    map = null;
                }

            World.saveLog.takeLine("Start deserial");
            string serializedState = File.ReadAllText(filename);
                fsSerializer _serializer = new fsSerializer();
                fsData data = fsJsonParser.Parse(serializedState);
                object deserialized = null;
                _serializer.TryDeserialize(data, typeof(Map), ref deserialized).AssertSuccessWithoutWarnings();
                World.saveLog.takeLine("Finished deserial");
                map = (Map)deserialized;
                map.world = this;
                staticMap = map;
                GraphicalMap.map = map;
                ui.setToMainMenu();
                GraphicalMap.checkLoaded();
                GraphicalMap.checkData();
                //GraphicalMap.loadArea(0, 0);
                World.Log("reached end of loading code");
                prefabStore.popMsg("Load may well have succeeded.");
            //}
            //catch (FileLoadException e)
            //{
            //    Debug.Log(e);
            //}
            //catch (Exception e2)
            //{
            //    Debug.Log(e2);
            //}
        }
    }

}
