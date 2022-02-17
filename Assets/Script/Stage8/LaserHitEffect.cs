using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserHitEffect : MonoBehaviour
{
    [SerializeField] LaserLauncher laserLauncher;

    public void StopHitEffect()
    {
        Invoke("Launch", 7f);
        gameObject.SetActive(false);
    }

    void Launch()
    {
        laserLauncher.Launch();
    }
}
