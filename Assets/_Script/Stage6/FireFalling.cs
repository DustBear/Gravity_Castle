using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireFalling : MonoBehaviour
{
    [SerializeField] ParticleSystem fireParticle;
    [SerializeField] ParticleSystem sparkParticle;
    [SerializeField] ParticleSystem smokeParticle;

    GameObject player;
    Scene originScene;
    Rigidbody2D rigid;
    BoxCollider2D collid;
    bool isFinish;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collid = GetComponent<BoxCollider2D>();
    }

    void OnEnable()
    {
        originScene = SceneManager.GetActiveScene();
        if (GameManager.instance.gameData.curAchievementNum >= 0)
        {
            player = GameObject.FindWithTag("Player");
        }
        rigid.gravityScale = 1f;
        rigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
        collid.isTrigger = false;
        isFinish = false;
    }

    void Update()
    {
        // If scene changes, then erase all activated fires
        if (originScene != SceneManager.GetActiveScene() || GameManager.instance.gameData.SpawnSavePoint_bool)
        {
            ObjManager.instance.ReturnObj(ObjManager.ObjType.fireFalling, gameObject);
        }
        else
        {
            transform.rotation = player.transform.rotation;
        }

        if (Player.curState == Player.States.ChangeGravityDir)
        {
            rigid.gravityScale = 0f;
            rigid.velocity = Vector2.zero;
        }
        else
        {
            rigid.gravityScale = 1f;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (!isFinish && other != null)
        {
            isFinish = true;
            if (other.gameObject.CompareTag("IcePlatform"))
            {
                StartCoroutine(EraseFireOnIce());
            }
            else
            {
                StartCoroutine(EraseFire());
            }
        }
    }

    IEnumerator EraseFireOnIce()
    {
        fireParticle.Stop();
        sparkParticle.Stop();
        smokeParticle.Stop();
        yield return new WaitForSeconds(0.4f); // Wait until fire particle goes on
        ObjManager.instance.ReturnObj(ObjManager.ObjType.fireFalling, gameObject);
    }

    IEnumerator EraseFire()
    {
        fireParticle.Stop();
        sparkParticle.Stop();
        smokeParticle.Stop();
        yield return new WaitForSeconds(0.4f); // Wait until fire particle goes on
        rigid.constraints = RigidbodyConstraints2D.FreezePosition;
        collid.isTrigger = true;
        yield return new WaitForSeconds(3.0f);
        rigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
        collid.isTrigger = false;
        ObjManager.instance.ReturnObj(ObjManager.ObjType.fireFalling, gameObject);
    }
}
