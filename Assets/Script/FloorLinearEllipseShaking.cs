using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorLinearEllipseShaking : MonoBehaviour
{
    public int floorNum;
    public float shakeRange;
    public float shakeDuration;
    public float waitingTime;
    MainCamera mainCamera;
    Player player;
    Rigidbody2D rigid;
    float time;
    enum State {idle, waiting, shaked, falling, fading};
    State state;
    SpriteRenderer sprite;
    
    public float[] speed;
    public Vector2[] pos;
    public int targetPosIdx;

    float movingTime;
    public float startMovingTime;
    Vector2 centerPos;
    Vector2 ellipsePos;
    public float ellipseSpeed;
    public float xRad, yRad;
    public bool counterClockWise;

    void Awake()
    {
        if (GameManager.instance.curIsShaked[floorNum]) {
            Destroy(gameObject);
        }
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<MainCamera>();
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
        state = State.idle;
        transform.position = pos[(targetPosIdx + pos.Length - 1) % pos.Length];
        movingTime = startMovingTime;
        centerPos = pos[0];
        sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        switch (state) {
            case State.idle:
                // linear moved position
                centerPos = Vector2.MoveTowards(centerPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
                // add ellipse moved position
                movingTime += Time.deltaTime;
                if (!counterClockWise) {
                    transform.position = centerPos + new Vector2(xRad * Mathf.Sin(movingTime * ellipseSpeed), yRad * Mathf.Cos(movingTime * ellipseSpeed));
                }
                else {
                    transform.position = centerPos + new Vector2(xRad * Mathf.Sin(-movingTime * ellipseSpeed), yRad * Mathf.Cos(-movingTime * ellipseSpeed));
                }
                // if floor arrive target position, convert targetPosIdx to next targetPosIdx
                if (centerPos == pos[targetPosIdx]) {
                    targetPosIdx = (targetPosIdx + 1) % pos.Length;
                }
                // if player is on the floor, move state to "shaked"
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
                    GameManager.instance.curIsShaked[floorNum] = true;
                    state = State.waiting;
                }
                break;
            case State.waiting:
                time += Time.deltaTime;
                centerPos = Vector2.MoveTowards(centerPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
                
                movingTime += Time.deltaTime;
                if (!counterClockWise) {
                    transform.position = centerPos + new Vector2(xRad * Mathf.Sin(movingTime * ellipseSpeed), yRad * Mathf.Cos(movingTime * ellipseSpeed));
                }
                else {
                    transform.position = centerPos + new Vector2(xRad * Mathf.Sin(-movingTime * ellipseSpeed), yRad * Mathf.Cos(-movingTime * ellipseSpeed));
                }
                
                if (centerPos == pos[targetPosIdx]) {
                    targetPosIdx = (targetPosIdx + 1) % pos.Length;
                }
                
                if (time >= waitingTime) {
                    time = 0;
                    state = State.shaked;
                }
                break;
            case State.shaked:
                time += Time.deltaTime;
                centerPos = Vector2.MoveTowards(centerPos, pos[targetPosIdx], speed[targetPosIdx] * Time.deltaTime);
                
                movingTime += Time.deltaTime;
                
                if (!counterClockWise) {
                    ellipsePos = centerPos + new Vector2(xRad * Mathf.Sin(movingTime * ellipseSpeed), yRad * Mathf.Cos(movingTime * ellipseSpeed));
                }
                else {
                    ellipsePos = centerPos + new Vector2(xRad * Mathf.Sin(-movingTime * ellipseSpeed), yRad * Mathf.Cos(-movingTime * ellipseSpeed));
                }

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
                transform.position = ellipsePos + new Vector2(mainCamera.shakedX, mainCamera.shakedY);
                
                
                if (centerPos == pos[targetPosIdx]) {
                    targetPosIdx = (targetPosIdx + 1) % pos.Length;
                }
                if (time >= shakeDuration) {
                    mainCamera.shakedX = 0f;
                    mainCamera.shakedY = 0f;
                    transform.position = ellipsePos;
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
                    StartCoroutine(FadeOut());
                }
                break;
            case State.fading:
                break;
        }
    }

    IEnumerator FadeOut() {
        for (float f = 1.0f; f >= 0.0f; f -= 0.1f) {
            Color color = sprite.material.color;
            color.a = f;
            sprite.material.color = color;
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < transform.childCount; i++) {
            Transform trans = transform.GetChild(i);
            if (trans.CompareTag("Player")) {
                trans.parent = null;
            }
        }
        Destroy(gameObject);
    }
}
