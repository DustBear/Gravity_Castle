using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Animator animator;
    GameManager gameManager;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        gameManager = GameObject.FindWithTag("GameController").GetComponent<GameManager>();
    }
    void Start() {
        rigid.AddForce(transform.up * 10.0f, ForceMode2D.Impulse);
        animator.SetBool("allowKey", true);
    }
    
    void Update() {
        if (rigid.velocity.y < 0) {
            sprite.sortingLayerName = "Key";
        }
        if (gameObject.CompareTag("Key1") && gameManager.isGetKey1
        || gameObject.CompareTag("Key2") && gameManager.isGetKey2) {
            Destroy(gameObject);
        }
    }
}
