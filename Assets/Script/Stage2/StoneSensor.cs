using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSensor : MonoBehaviour
{
    [SerializeField] Rigidbody2D stoneRigid;
    [SerializeField] float shakeRange;
    [SerializeField] float shakeDuration;
    bool isShaked;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isShaked && GameManager.instance.gameData.curAchievementNum < 17 && other.CompareTag("Player"))
        {
            StartCoroutine(StartShake());
        }
    }

    IEnumerator StartShake() {
        isShaked = true;
        float stopShakingTime = Time.time + shakeDuration;
        Vector2 stonePos = stoneRigid.transform.position;
        while (Time.time < stopShakingTime)
        {
            stoneRigid.transform.position = stonePos + Vector2.right * shakeRange;
            shakeRange *= -1f;
            yield return null;
        }
        stoneRigid.transform.position = stonePos;
        stoneRigid.gravityScale = 3f;
    }
}
