using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    [SerializeField] CameraShakeStage8 cameraShake;
    [SerializeField] PlayerStage8 player;
    [SerializeField] float shakeDir;
    [SerializeField] float targetDir;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!cameraShake.isShaked && !player.isDevilRotating && other != null && other.tag == "Player")
        {
            cameraShake.StartShaking(shakeDir, targetDir);
        }
    }
}
