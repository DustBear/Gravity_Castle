using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    static bool isAppQuit;

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
            }
            return _instance;
        }
    }

    void OnApplicationQuit()
    {
        isAppQuit = true;
    }

    void OnDestroy() {
        isAppQuit = true;
    }
}
