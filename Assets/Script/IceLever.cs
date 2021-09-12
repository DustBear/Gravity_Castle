using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceLever : MonoBehaviour
{
    bool isStart;
    SpriteRenderer iceRenderer;
    GameObject fire;

    void Awake() {
        iceRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!isStart && other.gameObject.CompareTag("Projectile")) {
            fire = GameManager.instance.fireQueue.Dequeue();
            fire.transform.position = transform.position;
            fire.SetActive(true);

            StartCoroutine(Melt());
            isStart = true;
        }
    }

    IEnumerator Melt() {
        while (iceRenderer.color.a > 0f) {
            Color iceColor = iceRenderer.color;
            if (iceColor.a > 0f) {
                iceColor.a -= 0.025f;
                iceRenderer.color = iceColor;
            }
            yield return new WaitForSeconds(0.1f);
        }
        // after melting
        fire.GetComponent<FireNotFalling>().isIceMelted = true;
        StartCoroutine(Wait());
    }

    IEnumerator Wait() {
        yield return new WaitForSeconds(1.0f);
        fire.SetActive(false);
        GameManager.instance.fireQueue.Enqueue(fire);
        Destroy(gameObject);
    }
}
