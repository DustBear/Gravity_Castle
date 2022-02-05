using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    Player player;
    RaycastHit2D rayHitWallUp, rayHitWallDown, rayHitPlatform, rayHitPlayerUp;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    SpriteRenderer spriteLever;
    bool isLeft;
    float sizeX, sizeY;
    bool isRotating;
    public float speed;
    public enum startMoving {dft, key1, door1, key2};
    public startMoving startMv;

    public GameObject missile;
    bool isMoving;
    bool isCollide;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        sizeX = transform.localScale.x;
        sizeY = transform.localScale.y;
    }

    void Start() {
        StartCoroutine(Moving());
        StartCoroutine(Shot());
        AutoChangeDir();
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider != null) {
            isCollide = true;
        }
    }

    void OnCollisionExit2D(Collision2D other) {
        if (other.collider != null) {
            isCollide = false;
        }
    }

    IEnumerator Moving() {
        switch (startMv) {
            case startMoving.key1:
                while (GameManager.instance.curAchievementNum < 25) {
                    yield return new WaitForSeconds(0.5f);
                }
                break;
            case startMoving.door1:
                while (GameManager.instance.curAchievementNum < 26) {
                    yield return new WaitForSeconds(0.5f);
                }
                break;
            case startMoving.key2:
                while (GameManager.instance.curAchievementNum < 27) {
                    yield return new WaitForSeconds(0.5f);
                }
                break;
            default:
                yield return new WaitForSeconds(0.5f);
                break;
        }
        rigid.bodyType = RigidbodyType2D.Kinematic;
        isMoving = true;
        while (player.leveringState != Player.LeveringState.changeGravityDir && rayHitPlayerUp.collider == null) {
            // Move
            transform.position = transform.position + transform.right * (isLeft ? -speed : speed) * Time.deltaTime;
            
            // Change direction
            if (isLeft) {
                rayHitWallUp = Physics2D.Raycast(transform.position + transform.up * sizeY * 0.4f - transform.right * sizeX * 0.6f, -transform.right, 0.1f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitWallDown = Physics2D.Raycast(transform.position - transform.up * sizeY * 0.4f - transform.right * sizeX * 0.6f, -transform.right, 0.1f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                //rayHitWall = Physics2D.BoxCast(transform.position - transform.right * sizeX * 0.6f, new Vector2(0.05f, sizeY * 0.2f), transform.rotation.z, -transform.right, 0f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitPlatform = Physics2D.Raycast(transform.position - transform.right * sizeX * 0.5f, -transform.up, sizeY * 0.6f, ~(1 << 18));
            }
            else {
                rayHitWallUp = Physics2D.Raycast(transform.position + transform.up * sizeY * 0.4f - transform.right * sizeX * 0.6f, -transform.right, 0.1f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitWallDown = Physics2D.Raycast(transform.position - transform.up * sizeY * 0.4f - transform.right * sizeX * 0.6f, -transform.right, 0.1f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                //rayHitWall = Physics2D.BoxCast(transform.position + transform.right * sizeX * 0.6f, new Vector2(0.05f, sizeY * 0.2f), transform.rotation.z, transform.right, 0f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitPlatform = Physics2D.Raycast(transform.position + transform.right * sizeX * 0.5f, -transform.up, sizeY * 0.6f, ~(1 << 18));
            }
            if (rayHitWallUp.collider != null || rayHitWallDown.collider != null || rayHitPlatform.collider == null) {
                isLeft = !isLeft;
            }
            else if (Mathf.Abs(Vector2.Dot(transform.up, transform.position - player.transform.position)) < 1f && Vector2.Distance(transform.position, player.transform.position) < 5f) {
                if ((isLeft && Vector2.Dot(-transform.right, transform.position - player.transform.position) > 0)
                || (!isLeft && Vector2.Dot(transform.right, transform.position - player.transform.position) > 0)) {
                    isLeft = !isLeft;
                }
            }
            rayHitPlayerUp = Physics2D.BoxCast(transform.position, new Vector2(sizeX, sizeY), transform.rotation.eulerAngles.z, transform.up, 0.1f, 1 << 10);
            yield return null;
        }
        if (rayHitPlayerUp.collider != null) {
            StartCoroutine(Destroyed());
        }
        else {
            StartCoroutine(Rotating());
        }
    }

    IEnumerator Rotating() {
        while (player.leveringState == Player.LeveringState.changeGravityDir) {
            transform.rotation = player.transform.rotation;
            yield return null;
        }
        StartCoroutine(Falling());
    }

    IEnumerator Falling() {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        while (!isCollide || Physics2D.Raycast(transform.position, -transform.up, sizeY * 0.6f, 1 << 3 | 1 << 19).collider == null) {
            yield return null;
        }
        rigid.velocity = Vector2.zero;
        StartCoroutine(Moving());
    }

    IEnumerator Destroyed() {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
        Invoke("Erase", 1f);
        while (true) {
            Color color = sprite.color;
            color.a = 0.5f - color.a;
            sprite.color = color;
            yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator Shot() {
        while (true) {
            if (isMoving && Vector2.Distance(transform.position, player.transform.position) < 10.0f
            && Mathf.Abs(Vector2.Dot(transform.up, transform.position - player.transform.position)) < 2f) {
                Instantiate(missile, transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(1.5f);
        }
    }

    void Erase() {
        Destroy(gameObject);
    }

    void AutoChangeDir() {
        isLeft = !isLeft;
        Invoke("AutoChangeDir", Random.Range(3, 10));
    }
}
