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
                // type 이름으로 찾기
                GameObject obj;
                obj = GameObject.Find(typeof(T).Name);
                // 없으면 만들기
                if (obj == null)
                {
                    obj = new GameObject(typeof(T).Name);
                    _instance = obj.AddComponent<T>();
                }
                // 있으면 GetComponent
                else
                {
                    _instance = obj.GetComponent<T>();
                }
            }
            return _instance;
        }
    }
}
