using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody2D rigid;
    public float firePower;
    public Vector2 fireDir;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Start() {
        rigid.AddForce(fireDir * firePower, ForceMode2D.Impulse);
    }

    void FixedUpdate() {
        float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
        transform.localEulerAngles = new Vector3(0, 0, 90 + angle * 180 / Mathf.PI);
    }

    void OnCollisionEnter2D(Collision2D other) {
        Destroy(gameObject);
    }
}
