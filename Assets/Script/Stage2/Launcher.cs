using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;
    [SerializeField] float timeInterval;
    [SerializeField] float nextTime;
    [SerializeField] float firePower;
    float startTime;
    float timeRemaining;

    void Awake() {
        startTime = Time.time;
    }

    void FixedUpdate() {
        if (timeRemaining == 0f && GameManager.instance.isChangeGravityDir)
        {
            // When player starts to change gravity direction, calculate time remaining until next launch
            timeRemaining = nextTime - Time.time + startTime;
        }
        else if (timeRemaining != 0f && !GameManager.instance.isChangeGravityDir)
        {
            // When playr finishes changing gravity direction, apply calculated time remaining to nextTime
            nextTime = Time.time - startTime + timeRemaining;
            timeRemaining = 0f;
        }

        if (!GameManager.instance.isChangeGravityDir)
        {
            float curTime = Time.time - startTime;
            if (curTime > nextTime)
            {
                nextTime = curTime + timeInterval;
                GameObject curObj = ObjManager.instance.GetObj(type);
                curObj.transform.position = transform.position;
                curObj.transform.rotation = transform.rotation;
                Projectile projectile = curObj.GetComponent<Projectile>();
                projectile.type = type;
                projectile.firePower = firePower;
                curObj.SetActive(true);
            }
        }
    }
}
