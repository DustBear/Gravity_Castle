using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Rigidbody2D rigid;
    Player player;
    Vector3 dir;

    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        dir = (player.transform.position - this.transform.position).normalized;
    }

    void Update() {
        this.transform.Translate(dir * Time.deltaTime * 5.0f, Space.World);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider != null) {
            Destroy(this);
        }
    }
}
