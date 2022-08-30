using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefault : MonoBehaviour
{
    Player player;
    RaycastHit2D rayHitWallDown, rayHitPlatform, rayHitPlayerUp;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    SpriteRenderer spriteLever;
    bool isLeft;
    float sizeX, sizeY;
    bool isRotating;
    bool isCollide;
    public float speed;
    public enum startMoving {dft, key1, door1, key2};
    public startMoving startMv;
    [SerializeField] int achievementNum;

    void Awake() {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        sizeX = transform.localScale.x;
        sizeY = transform.localScale.y;
    }

    void Start() {
        if (GameManager.instance.gameData.curAchievementNum > achievementNum)
        {
            Destroy(gameObject);
        }
        StartCoroutine(Moving());
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider != null) {
            isCollide = true;
            if (other.collider.tag == "Player" && rayHitPlayerUp.collider == null)
            {
                GameManager.instance.shouldSpawnSavePoint = true;
                UIManager.instance.FadeOut(1f);
            }
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
                while (GameManager.instance.gameData.curAchievementNum < 25) {
                    yield return new WaitForSeconds(0.5f);
                }
                break;
            case startMoving.door1:
                while (GameManager.instance.gameData.curAchievementNum < 26) {
                    yield return new WaitForSeconds(0.5f);
                }
                break;
            case startMoving.key2:
                while (GameManager.instance.gameData.curAchievementNum < 27) {
                    yield return new WaitForSeconds(0.5f);
                }
                break;
        }
        rigid.bodyType = RigidbodyType2D.Kinematic;
        while (Player.curState != Player.States.ChangeGravityDir && rayHitPlayerUp.collider == null) {
            // Move
            transform.position = transform.position + transform.right * (isLeft ? -speed : speed) * Time.deltaTime;
            
            // Change direction
            if (isLeft) {
                rayHitWallDown = Physics2D.Raycast(transform.position - transform.right * sizeX * 0.6f, -transform.right, 0.1f, 1 << 3 | 1 << 18 | 1 << 19);
                //rayHitWall = Physics2D.BoxCast(transform.position - transform.right * sizeX * 0.6f, new Vector2(0.05f, sizeY * 0.35f), transform.rotation.z, -transform.right, 0f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitPlatform = Physics2D.Raycast(transform.position - transform.right * sizeX * 0.5f, -transform.up, sizeY * 0.6f, ~(1 << 18));
            }
            else {
                rayHitWallDown = Physics2D.Raycast(transform.position + transform.right * sizeX * 0.6f, transform.right, 0.1f, 1 << 3 | 1 << 18 | 1 << 19);
                //rayHitWall = Physics2D.BoxCast(transform.position + transform.right * sizeX * 0.6f, new Vector2(0.05f, sizeY * 0.35f), transform.rotation.z, transform.right, 0f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitPlatform = Physics2D.Raycast(transform.position + transform.right * sizeX * 0.5f, -transform.up, sizeY * 0.6f, ~(1 << 18));
            }
            if (rayHitWallDown.collider != null || rayHitPlatform.collider == null) {
                isLeft = !isLeft;
            }
            // move toward player
            else if (Mathf.Abs(Vector2.Dot(transform.up, transform.position - player.transform.position)) < 1f && Vector2.Distance(transform.position, player.transform.position) < 5f) {
                if ((isLeft && Vector2.Dot(-transform.right, transform.position - player.transform.position) > 0)
                || (!isLeft && Vector2.Dot(transform.right, transform.position - player.transform.position) > 0)) {
                    isLeft = !isLeft;
                }
            }
            rayHitPlayerUp = Physics2D.BoxCast(transform.position, new Vector2(sizeX * 0.8f, 0.1f), transform.rotation.eulerAngles.z, transform.up, 0.7f, 1 << 10);
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
        while (Player.curState == Player.States.ChangeGravityDir) {
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
        Destroy(gameObject , 1f);
        while (true) {
            Color color = sprite.color;
            color.a = 0.5f - color.a;
            sprite.color = color;
            yield return new WaitForSeconds(0.02f);
        }
    }
}
