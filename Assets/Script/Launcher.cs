using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    public GameManager.Obj obj;
    GameManager gameManager;
    public float timeInterval;
    public float nextTime;
    float startTime;
    public float firePower;

    void Awake() {
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
        startTime = Time.time;
    }

    void FixedUpdate() {
        float curTime = Time.time - startTime;
        if (curTime > nextTime) {
            GameObject curObj = gameManager.MakeObj(obj);
            curObj.transform.position = transform.position;
            curObj.transform.rotation = transform.rotation;
            curObj.GetComponent<Projectile>().firePower = firePower;
            curObj.SetActive(true);
            nextTime = curTime + timeInterval;
        }
    }
}
