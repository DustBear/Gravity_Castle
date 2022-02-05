using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncher : MonoBehaviour
{
    [SerializeField] float timeInterval;

    void OnEnable()
    {
        StartCoroutine(LaunchFire());
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
}
