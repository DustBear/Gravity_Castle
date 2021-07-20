using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    public bool isAllowKey;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }
    void Start() {
        rigid.AddForce(transform.up * 10.0f, ForceMode2D.Impulse);
        StartCoroutine("BeforeGetKey");
    }

    IEnumerator BeforeGetKey() {
        while (transform.InverseTransformDirection(rigid.velocity).y >= 0) {
            yield return null;
        }
        sprite.sortingLayerName = "Key";
        isAllowKey = true;
        Physics2D.IgnoreLayerCollision(10, 13, false); // player, key
    }
}
