using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage8Intro : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] CameraShake cameraShake;

    void Start() {
        StartCoroutine(Intro());
    }

    IEnumerator Intro()
    {
        while (true)
        {
            yield return null;
            StartCoroutine(FirstShaking());
            
        }
    }

    IEnumerator FirstShaking()
    {
        while (player.transform.position.x <= -77f)
        {
            yield return null;
            if (player.transform.position.x >= -97f)
            {
                cameraShake.enabled = true;
            }
            else {
                cameraShake.enabled = false;
            }
        }
    }
}
