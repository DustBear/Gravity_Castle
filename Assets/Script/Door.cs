using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    SpriteRenderer sprite;
    BoxCollider2D collid;

    void Awake() {
        sprite = GetComponent<SpriteRenderer>();
        collid = GetComponent<BoxCollider2D>();
    }

    void Start() {
        if (gameObject.CompareTag("Door1") && GameManager.instance.isOpenDoor1
        || gameObject.CompareTag("Door2") && GameManager.instance.isOpenDoor2) {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Player")) {
            if (gameObject.CompareTag("Door1") && GameManager.instance.isGetKey1) {
                GameManager.instance.isOpenDoor1 = true;
                GameManager.instance.curState = 2;
                StartCoroutine("FadeOut");
            }
            else if (gameObject.CompareTag("Door2") && GameManager.instance.isGetKey2) {
                GameManager.instance.isOpenDoor2 = true;
                GameManager.instance.curState = 4;
                StartCoroutine("FadeOut");
            }
        }
    }

    IEnumerator FadeOut() {
        for (int i = 10; i >= 0; i--) {
            Color color = sprite.material.color;
            color.a = i / 10.0f;
            sprite.material.color = color;
            yield return new WaitForSeconds(0.1f);
        }
        collid.isTrigger = true;
    }
}