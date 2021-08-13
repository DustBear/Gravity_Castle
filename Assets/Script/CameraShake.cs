using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    MainCamera mainCamera;
    Player player;
    Vector3 cameraPos;
    public float shakeRange;
    public float shakeDuration;

    void Awake() {
        mainCamera = GetComponent<MainCamera>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start() {
        mainCamera.enabled = false;
        InvokeRepeating("StartShake", 0f, 0.005f);
        Invoke("StopShake", shakeDuration);
    }

    void StartShake() {
        Vector2 playerPos = player.transform.position;
        transform.position = new Vector3(playerPos.x + Random.value * shakeRange * 2 - shakeRange, playerPos.y + 3f + Random.value * shakeRange * 2 - shakeRange, -10f);
    }

    void StopShake() {
        CancelInvoke("StartShake");
        mainCamera.enabled = true;
        this.enabled = false;
    }
}
