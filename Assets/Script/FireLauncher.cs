using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncher : MonoBehaviour
{
    public float timeInterval;
    float startTime;
    float nextTime;

    void OnEnable() {
        startTime = Time.time;
        nextTime = 0f;
    }

    void FixedUpdate() {
        float curTime = Time.time - startTime;
        if (curTime > nextTime) {
            GameObject curObj = GameManager.instance.fireFallingQueue.Dequeue();
            curObj.transform.position = transform.position;
            curObj.SetActive(true);
            nextTime = curTime + timeInterval;
        }
    }
}
