using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    [SerializeField] ObjManager.ObjType type;
    [HideInInspector] public float firePower;
    [HideInInspector] public Vector2 fireDir;
    Rigidbody2D rigid;
    Scene originScene;
    bool curIsGravityChanged;
    
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnEnable() {
        originScene = SceneManager.GetActiveScene();
        if (type == ObjManager.ObjType.arrow)
        {
            rigid.AddForce(fireDir * firePower, ForceMode2D.Impulse);
        }
    }

    void Update() {
        // Erase projectile when scene is changed
        if (originScene != SceneManager.GetActiveScene() || GameManager.instance.shouldStartAtSavePoint)
        {
            ObjManager.instance.ReturnObj(type, gameObject);
        }

        // Move arrow
        if (type == ObjManager.ObjType.arrow)
        {
            if (Player.curState != Player.States.ChangeGravityDir)
            {
                // When gravity change is finished
                if (curIsGravityChanged)
                {
                    rigid.AddForce(Physics2D.gravity.normalized * firePower, ForceMode2D.Impulse);
                    rigid.gravityScale = 2f;
                    curIsGravityChanged = false;
                }

                float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x);
                transform.localEulerAngles = new Vector3(0, 0, 90f + angle * 180f / Mathf.PI);
            }
            // When gravity change is started
            else if (!curIsGravityChanged)
            {
                rigid.gravityScale = 0f;
                rigid.velocity = Vector2.zero;
                curIsGravityChanged = true;
            }
        }
        // Move cannon
        else if (type == ObjManager.ObjType.cannon && Player.curState != Player.States.ChangeGravityDir)
        {
            transform.position += transform.up * 10f * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        ObjManager.instance.ReturnObj(type, gameObject);
    }
}
