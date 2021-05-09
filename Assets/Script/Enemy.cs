using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Player player;
    Rigidbody2D rigid;
    public float firePower;
    public float timeLimit;
    float time;
    
    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start() {
        rigid.AddForce(Vector2.left * firePower, ForceMode2D.Impulse);
    }

    void Update() {
        if (time > timeLimit || player.rotateDirection != 0) {
            Destroy(gameObject);
        }
        time += Time.deltaTime;
    }
}
