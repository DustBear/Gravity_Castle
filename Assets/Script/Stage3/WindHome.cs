using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindHome : MonoBehaviour
{
    [SerializeField] bool isToggle;
    [SerializeField] float startTime;
    [SerializeField] float activeTime;
    [SerializeField] float inActiveTime;
    [SerializeField] GameObject windZone;
    [SerializeField] ParticleSystem windParticle;
    Animator windAnimator;
    
    void Awake() {
        windAnimator = GetComponent<Animator>();
    }

    void Start() {
        if (isToggle)
        {
            StartCoroutine(ToggleWind());
        }
        else
        {
            windAnimator.SetBool("isActive", true);
            windZone.SetActive(true);
            windParticle.Play();
        }
    }

    IEnumerator ToggleWind()
    {
        yield return new WaitForSeconds(startTime);
        while (true)
        {
            if (!windZone.activeSelf)
            {
                windAnimator.SetBool("isActive", true);
                windZone.SetActive(true);
                windParticle.Play();
                yield return new WaitForSeconds(activeTime);
            }
            else
            {
                windAnimator.SetBool("isActive", false);
                windZone.SetActive(false);
                windParticle.Stop();
                yield return new WaitForSeconds(inActiveTime);
            }
        }
    }
}
