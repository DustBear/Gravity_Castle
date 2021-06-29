using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    Rigidbody2D rigid;
    public float firePower;
    public Vector2 fireDir;
    public Quaternion fireRot;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start() {
        transform.rotation = fireRot;
        rigid.AddForce(fireDir * firePower, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D other) {
        Destroy(gameObject);
    }
}
