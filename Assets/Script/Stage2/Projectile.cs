using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public ObjManager.ObjType type;
    [HideInInspector] public float firePower;
    Rigidbody2D rigid;
    Scene originScene;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable() {
        originScene = SceneManager.GetActiveScene();
        rigid.AddForce(transform.up * firePower, ForceMode2D.Impulse);
        StartCoroutine(StopWhileChangingGravityDir());
    }

    void Update() {
        if (originScene != SceneManager.GetActiveScene()) {
            gameObject.SetActive(false);
            ObjManager.instance.ReturnObj(type, gameObject);
        }
        if (type == ObjManager.ObjType.arrow && !GameManager.instance.isChangeGravityDir) {
            float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
            transform.localEulerAngles = new Vector3(0, 0, 90f + angle * 180f / Mathf.PI);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        gameObject.SetActive(false);
        ObjManager.instance.ReturnObj(type, gameObject);
    }

    IEnumerator StopWhileChangingGravityDir()
    {
        while (true)
        {
            while (!GameManager.instance.isChangeGravityDir)
            {
                yield return null;
            }
            Vector2 storeVel = rigid.velocity;
            rigid.velocity = Vector2.zero;
            while (GameManager.instance.isChangeGravityDir)
            {
                yield return null;
            }
            rigid.velocity = storeVel;
        }
    }
}
