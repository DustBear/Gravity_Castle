using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindHome : MonoBehaviour
{
    [SerializeField] ParticleSystem windParticle;
    Animator windAnimator;
    
    void Awake() {
        windAnimator = GetComponent<Animator>();
    }

    void OnEnable() {
        windAnimator.SetBool("isActive", true);
        windParticle.Play();
    }
}
