using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigid;
    public float firePower;
    public bool isArrow;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable() {
        Vector2 fireDir;
        switch (transform.eulerAngles.z) {
            case 90:
                fireDir = new Vector2(-1, 0);
                break;
            case 180:
                fireDir = new Vector2(0, -1);
                break;
            case 270:
                fireDir = new Vector2(1, 0);
                break;
            default:
                fireDir = new Vector2(0, 1);
                break;
        }
        rigid.AddForce(fireDir * firePower, ForceMode2D.Impulse);
    }

    void FixedUpdate() {
        if (isArrow) {
            float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
            transform.localEulerAngles = new Vector3(0, 0, 90 + angle * 180 / Mathf.PI);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        gameObject.SetActive(false);
    }
}
