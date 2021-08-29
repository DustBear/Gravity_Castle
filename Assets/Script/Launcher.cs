using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public GameManager.Type type;
    public float timeInterval;
    public float nextTime;
    float startTime;
    public float firePower;

    void Awake() {
        startTime = Time.time;
    }

    void FixedUpdate() {
        float curTime = Time.time - startTime;
        if (curTime > nextTime) {
            GameObject curObj;
            if (type == GameManager.Type.arrow) {
                curObj = GameManager.instance.arrowQueue.Dequeue();
            }
            else {
                curObj = GameManager.instance.cannonQueue.Dequeue();
            }
            curObj.transform.position = transform.position;
            curObj.transform.rotation = transform.rotation;
            Projectile projectile = curObj.GetComponent<Projectile>();
            projectile.type = type;
            projectile.firePower = firePower;
            curObj.SetActive(true);
            nextTime = curTime + timeInterval;
        }
    }
}
