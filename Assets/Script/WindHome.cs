using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindHome : MonoBehaviour
{
    public bool isToggle;
    public float startTime;
    public float activeTime;
    public float inActiveTime;
    GameObject windZone;
    float time;
    bool isStart;
    Animator windAnimator;
    
    void Awake() {
        windZone = transform.GetChild(0).gameObject;
        windAnimator = GetComponent<Animator>();
    }

    void Start() {
        if (!isToggle) {
            windAnimator.SetBool("isActive", true);
            windZone.SetActive(true);
            this.enabled = false;
        }
        StartCoroutine("StartWind");
    }

    void FixedUpdate() {
        if (isStart) {
            time += Time.deltaTime;
            if (!windZone.activeSelf) {
                if (time >= inActiveTime) {
                    windAnimator.SetBool("isActive", true);
                    windZone.SetActive(true);
                    time = 0;
                }
            }
            else {
                if (time >= activeTime) {
                    windAnimator.SetBool("isActive", false);
                    windZone.SetActive(false);
                    time = 0;
                }
            }
        }
    }

    IEnumerator StartWind() {
        yield return new WaitForSeconds(startTime);
        windAnimator.SetBool("isActive", true);
        windZone.SetActive(true);
        time = 0;
        isStart = true;
    }
}
