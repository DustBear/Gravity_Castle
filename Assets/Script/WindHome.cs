using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindHome : MonoBehaviour
{
    public float startTime;
    public float activeTime;
    public float inActiveTime;
    GameObject windZone;
    float time;
    bool isStart;
    
    void Awake() {
        windZone = transform.GetChild(0).gameObject;
    }

    void Start() {
        StartCoroutine("StartWind");
    }

    void FixedUpdate() {
        if (isStart) {
            time += Time.deltaTime;
            if (!windZone.activeSelf) {
                if (time >= inActiveTime) {
                    windZone.SetActive(true);
                    time = 0;
                }
            }
            else {
                if (time >= activeTime) {
                    windZone.SetActive(false);
                    time = 0;
                }
            }
        }
    }

    IEnumerator StartWind() {
        yield return new WaitForSeconds(startTime);
        windZone.SetActive(true);
        time = 0;
        isStart = true;
    }
}
