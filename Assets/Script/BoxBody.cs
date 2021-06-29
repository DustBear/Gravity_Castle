using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBody : MonoBehaviour
{
    Animator anim;
    public bool isGetKey;

    void Awake() {
        anim = GetComponent<Animator>();
    }
    
    void Update() {
        
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            
        }
    }
    
}
