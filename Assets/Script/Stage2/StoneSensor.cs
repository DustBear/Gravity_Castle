using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSensor : MonoBehaviour
{
    [SerializeField] CameraShake cameraShake;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            cameraShake.enabled = true;
            this.enabled = false;
        }
    }
}
