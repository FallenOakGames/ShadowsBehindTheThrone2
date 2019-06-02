using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using FullSerializer;

namespace Assets.Code
{
    /*
     * World is your monobehaviour master. It calls the start function, triggering all loading and suchlikes and suchforths
     *
     * It has the ONLY reference to map. Map must be kept apart from any Unity gameObjects, so it can be serialised out
     * Obviously this is impossible, but every class which knows about its unity GameObject must be able to purge this reference
     *
     * It holds the references to the 'stores'. These are repositories which should not be serialised into the saved games
     */
    public class World : MonoBehaviour
    {
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

        public void Start()
        {
            //ui.setToMainMenu();
            startup();
            //ui.setToWorld();
        }

        public void Update()
        {
            staticMap = map;
        }

        public string pathPrefix = "";
        public bool isWindows = false;
        public void specificStartup()
        {
            if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.Windows)
            {
                Log("Windows! A windows OS");
                isWindows = true;
                pathPrefix = "";
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

            EconTrait.loadTraits(map);
            map.world = this;
            map.gen();

            for (int i = 0; i < param.burnInSteps; i++)
            {
                map.turnTick();
            }

            ui.setToWorld();
            Log("Got to end of initial startup");
        }

        public void bEndTurn()
        {
            if (turnLock) { return; }

            turnLock = true;
            if (map != null) {
                map.turnTick();
            }

            turnLock = false;
            ui.uiHex.checkData();
        }
        public void b10Turns()
        {
            if (turnLock) { return; }

            turnLock = true;
            if (map != null)
            {
                for (int i = 0; i < 10; i++)
                {
                    map.turnTick();
                }
            }

            turnLock = false;
            ui.uiHex.checkData();
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
        /*
        public void save(string filename)
        {
            World world = this;
            world.ui.setToBackground();
            GraphicalMap.purge();
            GraphicalSociety.purge();
            world.map.world = null;


            fsSerializer _serializer = new fsSerializer();
            fsData data;
            _serializer.TrySerialize(typeof(Map), map, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            string saveString = fsJsonPrinter.CompressedJson(data);
            World.Log("Save exit point");

            if (File.Exists(filename))
            {
                World.Log("Overwriting old save: " + filename);
                File.Delete(filename);
            }
            File.WriteAllLines(filename, new string[] { saveString });

            world.map.world = world;

            world.prefabStore.popMsg("Game saved as: " + filename);

            //// step 1: parse the JSON data
            //fsData data = fsJsonParser.Parse(serializedState);

            //// step 2: deserialize the data
            //object deserialized = null;
            //_serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();
        }

        public void load(string filename)
        {
            try
            {
                if (map != null)
                {
                    GraphicalMap.purge();
                    GraphicalSociety.purge();
                    map.world = null;
                    map = null;
                }

                string serializedState = File.ReadAllText(filename);
                fsSerializer _serializer = new fsSerializer();
                fsData data = fsJsonParser.Parse(serializedState);
                object deserialized = null;
                _serializer.TryDeserialize(data, typeof(Map), ref deserialized).AssertSuccessWithoutWarnings();
                map = (Map)deserialized;
                map.world = this;
                map.makeGrid();
                commonStartup();
                GraphicalMap.map = map;
                GraphicalSociety.map = map;
                //GraphicalMap.checkLoaded();
                //GraphicalMap.checkData();
                //graphicalMap.loadArea(0, 0);
                World.Log("reached end of loading code");
                prefabStore.popMsg("Load may well have succeeded.");
            }
            catch (FileLoadException e)
            {
                Debug.Log(e);
            }
            catch (Exception e2)
            {
                Debug.Log(e2);
            }
        }
        */
    }

}
