using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireFalling : MonoBehaviour
{
    Scene originScene;
    ParticleSystem fireParticle, sparkParticle, smokeParticle;
    Rigidbody2D rigid;
    BoxCollider2D collid;
    Player player;
    bool isFinish;
    bool isApplyRotating;
    
    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        fireParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        sparkParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
        smokeParticle = transform.GetChild(2).GetComponent<ParticleSystem>();
        rigid = GetComponent<Rigidbody2D>();
        collid = GetComponent<BoxCollider2D>();
    }

    void OnEnable() {
        originScene = SceneManager.GetActiveScene();
        isFinish = false;
        rigid.gravityScale = 1f;
        collid.isTrigger = false;
    }

    void Update() {
        transform.rotation = player.transform.rotation;

        if (isApplyRotating) {
            rigid.gravityScale = 0f;
            rigid.velocity = Vector2.zero;
        }
        else {
            rigid.gravityScale = 1f;
        }
        
        // if scene changes, then erase all activated fires
        if (originScene != SceneManager.GetActiveScene()) {
            gameObject.SetActive(false);
            GameManager.instance.fireFallingQueue.Enqueue(gameObject);
        }

        // change isApplyRotating to fit isRotating
        if (!isApplyRotating && GameManager.instance.isRotating) {
            isApplyRotating = true;
        }
        if (isApplyRotating && !GameManager.instance.isRotating) {
            isApplyRotating = false;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!isFinish && other != null) {
            isFinish = true;
            if (other.gameObject.CompareTag("IcePlatform")) {
                StartCoroutine(EraseFireOnIce());
            }
            else {
                StartCoroutine(EraseFire());
            }
        }
    }

    IEnumerator EraseFireOnIce() {
        fireParticle.Stop();
        sparkParticle.Stop();
        smokeParticle.Stop();
        yield return new WaitForSeconds(0.4f); // wait until fire particle goes on
        gameObject.SetActive(false);
        GameManager.instance.fireFallingQueue.Enqueue(gameObject);
    }

    IEnumerator EraseFire() {
        fireParticle.Stop();
        sparkParticle.Stop();
        smokeParticle.Stop();
        yield return new WaitForSeconds(0.4f); // wait until fire particle goes on
        rigid.constraints = RigidbodyConstraints2D.FreezePosition;
        collid.isTrigger = true;
        yield return new WaitForSeconds(3.0f);
        rigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
        collid.isTrigger = false;
        gameObject.SetActive(false);
        GameManager.instance.fireFallingQueue.Enqueue(gameObject);
    }
}
