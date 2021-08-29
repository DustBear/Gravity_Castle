using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FireFalling : MonoBehaviour
{
    Scene originScene;
    ParticleSystem particle;
    Rigidbody2D rigid;
    BoxCollider2D collid;
    bool isFinish;
    bool isApplyRotating;
    
    void Awake() {
        particle = GetComponent<ParticleSystem>();
        rigid = GetComponent<Rigidbody2D>();
        collid = GetComponent<BoxCollider2D>();
    }

    void OnEnable() {
        originScene = SceneManager.GetActiveScene();
        isFinish = false;
        collid.isTrigger = false;
    }

    void Update() {
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
                particle.Stop();
                StartCoroutine(EraseFire());
            }
        }
    }

    IEnumerator EraseFireOnIce() {
        yield return new WaitForSeconds(1.0f);
        particle.Stop();
        gameObject.SetActive(false);
        GameManager.instance.fireFallingQueue.Enqueue(gameObject);
    }

    IEnumerator EraseFire() {
        yield return new WaitForSeconds(0.2f);
        collid.isTrigger = true;
        yield return new WaitForSeconds(3.0f);
        gameObject.SetActive(false);
        GameManager.instance.fireFallingQueue.Enqueue(gameObject);
    }
}
