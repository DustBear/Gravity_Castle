using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    private static bool isAppQuit;

    public static T instance
    {
        get
        {
            if (isAppQuit)
            {
                return null;
            }
            
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    private void OnApplicationQuit()
    {
        isAppQuit = true;
    }

    private void OnDestroy() {
        isAppQuit = true;
    }
}
