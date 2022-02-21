using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeStage8 : MonoBehaviour
{
    [SerializeField] PlayerStage8 player;
    [SerializeField] float shakeDuration;
    [SerializeField] float shakeRange;
    MainCamera mainCamera;
    [HideInInspector] public bool isShaked;

    void Awake()
    {
        mainCamera = GetComponent<MainCamera>();
    }

    public void StartShaking(float shakeDir, float targetDir)
    {
        StartCoroutine(Shake(shakeDir, targetDir));
    }

    IEnumerator Shake(float shakeDir, float targetDir)
    {
        isShaked = true;
        float shakeDirDelta = shakeDir > 0f ? 0.003f : -0.003f;
        float stopShakingTime = Time.time + shakeDuration;
        while (Time.time < stopShakingTime)
        {
            mainCamera.shakedX = Random.value * shakeRange * 2 - shakeRange;
            mainCamera.shakedY = Random.value * shakeRange * 2 - shakeRange;
            mainCamera.shakedZ = Random.value * shakeDir;
            shakeDir += shakeDirDelta;
            yield return new WaitForSeconds(0.005f);
        }

        mainCamera.shakedX = 0f;
        mainCamera.shakedY = 0f;
        mainCamera.shakedZ = 0f;
        StartCoroutine(player.DevilUsingLever(targetDir));
        isShaked = false;
    }
}
