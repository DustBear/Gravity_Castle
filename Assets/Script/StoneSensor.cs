using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSensor : MonoBehaviour
{
    CameraShake cameraShake;
    bool isSensed;

    void Awake() {
        cameraShake = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !isSensed) {
            isSensed = true;
            cameraShake.enabled = true;
            StartCoroutine(ActivateStone());
        }
    }

    IEnumerator ActivateStone() {
        while (cameraShake.isActiveAndEnabled) {
            yield return null;
        }
        transform.parent.GetChild(0).gameObject.SetActive(true);
        this.enabled = false;
    }
}
