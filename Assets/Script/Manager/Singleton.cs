using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T _instance;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                // type �̸����� ã��
                GameObject obj;
                obj = GameObject.Find(typeof(T).Name);
                // ������ �����
                if (obj == null)
                {
                    obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
                // ������ GetComponent
                else
                {
                    _instance = obj.GetComponent<T>();
                }
            }
            return _instance;
        }
    }
}
