using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    Rigidbody2D rigid;
    public GameManager.Type type;
    public float firePower;
    public bool isArrow;
    Scene originScene;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable() {
        originScene = SceneManager.GetActiveScene();
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

    void Update() {
        if (originScene != SceneManager.GetActiveScene()) {
            EraseProjectile();
        }
        if (isArrow) {
            float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
            transform.localEulerAngles = new Vector3(0, 0, 90 + angle * 180 / Mathf.PI);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        EraseProjectile();
    }

    void EraseProjectile() {
        if (type == GameManager.Type.arrow) {
            GameManager.instance.arrowQueue.Enqueue(gameObject);
        }
        else {
            GameManager.instance.cannonQueue.Enqueue(gameObject);
        }
        gameObject.SetActive(false);
    }
}
