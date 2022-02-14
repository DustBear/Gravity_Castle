using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    [SerializeField] CameraShakeStage8 cameraShake;
    [SerializeField] float shakeDir;
    [SerializeField] float targetDir;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.tag == "Player")
        {
            cameraShake.StartShaking(shakeDir, targetDir);
        }
    }
}
