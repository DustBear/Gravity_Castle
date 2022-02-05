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
    Rigidbody2D rigid;
    BoxCollider2D collid;
    Scene originScene;
    bool isFinish;
    bool isApplyRotating;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        collid = GetComponent<BoxCollider2D>();
    }

    void OnEnable()
    {
        if (GameManager.instance.curAchievementNum >= 0)
        {
            player = GameObject.FindWithTag("Player");
        }
        rigid.gravityScale = 1f;
        collid.isTrigger = false;
        originScene = SceneManager.GetActiveScene();
        isFinish = false;
    }

    void Update()
    {
        transform.rotation = player.transform.rotation;

        if (isApplyRotating)
        {
            rigid.gravityScale = 0f;
            rigid.velocity = Vector2.zero;
        }
        else
        {
            rigid.gravityScale = 1f;
        }

        // If scene changes, then erase all activated fires
        if (originScene != SceneManager.GetActiveScene())
        {
            gameObject.SetActive(false);
            ObjManager.instance.ReturnObj(ObjManager.ObjType.fireFalling, gameObject);
        }

        // Change isApplyRotating to fit isRotating
        if (!isApplyRotating && GameManager.instance.isChangeGravityDir)
        {
            isApplyRotating = true;
        }
        if (isApplyRotating && !GameManager.instance.isChangeGravityDir)
        {
            isApplyRotating = false;
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
        gameObject.SetActive(false);
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
        gameObject.SetActive(false);
        ObjManager.instance.ReturnObj(ObjManager.ObjType.fireFalling, gameObject);
    }
}
