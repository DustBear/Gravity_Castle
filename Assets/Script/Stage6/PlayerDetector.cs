using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] bool isLasting;
    bool isFinish;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isFinish && other.CompareTag("Player"))
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
                    transform.GetChild(i).GetComponent<FireLauncherLasting>().enabled = true;
                }
            }
            isFinish = true;
        }
    }
}
