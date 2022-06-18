using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncher : MonoBehaviour
{
    [SerializeField] float timeInterval;
    IEnumerator coroutine;

    void Awake()
    {
        coroutine = LaunchFire();
    }

    void OnEnable()
    {
        StartCoroutine(coroutine);
    }

    IEnumerator LaunchFire()
    {
        while (true)
        {
            GameObject fireFalling = ObjManager.instance.GetObj(ObjManager.ObjType.fireFalling);
            fireFalling.transform.position = transform.position;
            fireFalling.SetActive(true);
            yield return new WaitForSeconds(timeInterval);
        }
    }

    void OnDisable() {
        StopCoroutine(coroutine);
    }
}
