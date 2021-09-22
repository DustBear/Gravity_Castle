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
    ParticleSystem windParticle;
    float time;
    bool isStart;
    Animator windAnimator;
    
    void Awake() {
        windZone = transform.GetChild(0).gameObject;
        windParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        windAnimator = GetComponent<Animator>();
    }

    void Start() {
        if (!isToggle) {
            windAnimator.SetBool("isActive", true);
            windZone.SetActive(true);
            windParticle.Play();
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
                    windParticle.Play();
                    time = 0;
                }
            }
            else {
                if (time >= activeTime) {
                    windAnimator.SetBool("isActive", false);
                    windZone.SetActive(false);
                    windParticle.Stop();
                    time = 0;
                }
            }
        }
    }

    IEnumerator StartWind() {
        yield return new WaitForSeconds(startTime);
        windAnimator.SetBool("isActive", true);
        windZone.SetActive(true);
        windParticle.Play();
        time = 0;
        isStart = true;
    }
}
