using UnityEngine;
using UnityEditor;
using OdinSerializer;

namespace Assets.Code
{
    public class SAVE_MapsAndHexes : SerializedScriptableObject
    {
        public Hex[][] grid;
        public float[][] humidity;
        public float[][] temp;
    }
}