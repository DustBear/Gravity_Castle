using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireLauncher : MonoBehaviour
{
    public float timeInterval;
    public float nextTime;
    float startTime;

    void Awake() {
        startTime = Time.time;
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
