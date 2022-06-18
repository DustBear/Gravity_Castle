using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSensor : MonoBehaviour
{
    [SerializeField] CameraShakeStage8 cameraShake;
    [SerializeField] float shakeDir;
    [SerializeField] float targetDir;
    Player player;

    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!cameraShake.isShaked && !player.isDevilRotating && other != null && other.tag == "Player")
        {
            cameraShake.StartShaking(shakeDir, targetDir);
        }
    }
}
