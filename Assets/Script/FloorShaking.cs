using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorShaking : MonoBehaviour
{
    public float shakeRange;
    public float shakeDuration;
    public float waitingTime;
    MainCamera mainCamera;
    Rigidbody2D rigid;
    Vector2 floorPos;
    float time;
    enum State {idle, waiting, shaked, falling};
    State state;

    void Awake() {
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<MainCamera>();
        rigid = GetComponent<Rigidbody2D>();
        floorPos = transform.position;
        state = State.idle;
    }

    void Update() {
        switch (state) {
            case State.idle:
                RaycastHit2D rayHitPlayer;
                if (Physics2D.gravity == new Vector2(-9.8f, 0f)) {
                    rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x + transform.localScale.x / 2f + 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.7f), 0f, Vector2.right, 0.5f, 1 << 10);
                }
                else if (Physics2D.gravity == new Vector2(9.8f, 0f)) {
                    rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x - transform.localScale.x / 2f - 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.7f), 0f, Vector2.left, 0.5f, 1 << 10);
                }
                else if (Physics2D.gravity == new Vector2(0f, 9.8f)) {
                    rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2f - 0.1f), new Vector2(transform.localScale.x * 0.7f, 0.01f), 0f, Vector2.down, 0.5f, 1 << 10);
                }
                else {
                    rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2f + 0.1f), new Vector2(transform.localScale.x * 0.7f, 0.01f), 0f, Vector2.up, 0.5f, 1 << 10);
                }
                if (rayHitPlayer.collider != null) {
                    state = State.waiting;
                }
                break;
            case State.waiting:
                time += Time.deltaTime;
                if (time >= waitingTime) {
                    time = 0;
                    state = State.shaked;
                }
                break;
            case State.shaked:
                time += Time.deltaTime;
                if (Physics2D.gravity.x == 0) {
                    if (mainCamera.shakedX == 0f) {
                        mainCamera.shakedX = shakeRange;
                    }
                    else {
                        mainCamera.shakedX *= -1;
                    }
                    mainCamera.shakedY = 0f;
                }
                else {
                    mainCamera.shakedX = 0f;
                    if (mainCamera.shakedY == 0f) {
                        mainCamera.shakedY = shakeRange;
                    }
                    else {
                        mainCamera.shakedY *= -1;
                    }
                }
                transform.position = new Vector2(floorPos.x + mainCamera.shakedX, floorPos.y + mainCamera.shakedY);
                if (time >= shakeDuration) {
                    mainCamera.shakedX = 0f;
                    mainCamera.shakedY = 0f;
                    transform.position = floorPos;
                    if (Physics2D.gravity.x == 0) {
                        rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
                    }
                    else {
                        rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
                    }
                    rigid.bodyType = RigidbodyType2D.Dynamic;
                    state = State.falling;
                }
                break;
            case State.falling:
                RaycastHit2D rayHitPlatform;
                if (Physics2D.gravity == new Vector2(-9.8f, 0f)) {
                    rigid.constraints = ~RigidbodyConstraints2D.FreezePositionX;
                    rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
                    rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x - transform.localScale.x / 2f - 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.6f), 0f, Vector2.left, 0.5f);
                }
                else if (Physics2D.gravity == new Vector2(9.8f, 0f)) {
                    rigid.constraints = ~RigidbodyConstraints2D.FreezePositionX;
                    rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
                    rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x + transform.localScale.x / 2f + 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.6f), 0f, Vector2.right, 0.5f);
                }
                else if (Physics2D.gravity == new Vector2(0f, 9.8f)) {
                    rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
                    rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
                    rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2f + 0.1f), new Vector2(transform.localScale.x * 0.6f, 0.01f), 0f, Vector2.up, 0.5f);
                }
                else {
                    rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
                    rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
                    rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2f - 0.1f), new Vector2(transform.localScale.x * 0.6f, 0.01f), 0f, Vector2.down, 0.5f);
                }
                if (rayHitPlatform.collider != null) {
                    for (int i = 0; i < transform.childCount; i++) {
                        Transform trans = transform.GetChild(i);
                        if (trans.CompareTag("Player")) {
                            trans.parent = null;
                        }
                    }
                    Destroy(gameObject);
                }
                break;
        }
    }
}
