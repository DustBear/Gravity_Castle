using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceLever : MonoBehaviour
{
    bool isStart;
    SpriteRenderer iceRenderer, leverRenderer;
    Transform lever;

    void Awake() {
        iceRenderer = GetComponent<SpriteRenderer>();
        lever = transform.GetChild(0);
        leverRenderer = lever.gameObject.GetComponent<SpriteRenderer>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!isStart && other.CompareTag("Projectile")) {
            StartCoroutine(Melt());
            isStart = true;
        }
    }

    IEnumerator Melt() {
        while (iceRenderer.color.a > 0f || leverRenderer.color.r < 1f) {
            Color iceColor = iceRenderer.color;
            Color leverColor = leverRenderer.color;
            if (iceColor.a > 0f) {
                iceColor.a -= 0.01f;
                iceRenderer.color = iceColor;
            }
            if (leverColor.r < 1f) {
                leverColor.r += 0.025f;
                leverRenderer.color = leverColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
        lever.parent = null;
        Destroy(gameObject);
    }
}
