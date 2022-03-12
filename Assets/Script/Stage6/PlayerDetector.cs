using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] int detectorNum;
    [SerializeField] bool isLasting;
    bool isFinish;

    void Start()
    {
        if (GameManager.instance.curIsDetected[detectorNum])
        {
            ActivateLauncher(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFinish && other.CompareTag("Player"))
        {
            ActivateLauncher(false);
        }
    }

    void ActivateLauncher(bool isDetected)
    {
        if (!isLasting)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).GetComponent<FireLauncher>().enabled = true;
            }
        }
        else
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                FireLauncherLasting fireLauncherLasting = transform.GetChild(i).GetComponent<FireLauncherLasting>();
                if (isDetected)
                {
                    fireLauncherLasting.transform.position = fireLauncherLasting.finalPos;
                }
                fireLauncherLasting.enabled = true;
            }
        }
        GameManager.instance.curIsDetected[detectorNum] = true;
        isFinish = true;
    }
}
