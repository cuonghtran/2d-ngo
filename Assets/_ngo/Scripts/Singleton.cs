using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace BasicNetcode
{
    public class Singleton<T> : NetworkBehaviour where T : Component
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    var objs = FindObjectsOfType(typeof(T)) as T[];
                    if (objs.Length > 0)
                    {
                        _instance = objs[0];
                    }
                    if (objs.Length > 1)
                    {
                        Debug.LogError($"There is more than one {typeof(T).Name} in the scene.");
                    }
                    if (_instance == null)
                    {
                        GameObject go = new GameObject();
                        go.name = $"_{typeof(T).Name}";
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }
    }
}
