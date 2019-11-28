using OdinSerializer;
﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class SavableMap<T, T2> : SerializedScriptableObject
    {
        public List<T> keys = new List<T>();
        public List<T2> values = new List<T2>();

        public T2 lookup(T key){
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    return values[i];
                }
            }
            return default(T2);
        }

        internal void Clear()
        {
            keys.Clear();
            values.Clear();
        }

        public void Add(T key, T2 value)
        {
            add(key, value);
        }
        public void add(T key,T2 value)
        {
            keys.Add(key);
            values.Add(value);
        }

        public bool ContainsKey(T key)
        {
            return keys.Contains(key);
        }

        internal void set(T key, T2 value)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i].Equals(key))
                {
                    values[i] = value;
                    return;
                }
            }
            add(key, value);
        }
    }
}