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
        float shakeDirDelta = shakeDir > 0f ? 0.05f : -0.05f;
        float stopShakingTime = Time.time + shakeDuration;
        mainCamera.shakedZ = 0f;
        while (Time.time < stopShakingTime)
        {
            mainCamera.shakedZ += shakeDirDelta;
            yield return new WaitForSeconds(0.005f);
        }
        StartCoroutine(player.DevilUsingLever(targetDir));
        isShaked = false;
    }
}
