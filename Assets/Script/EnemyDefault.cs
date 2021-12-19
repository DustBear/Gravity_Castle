using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefault : MonoBehaviour
{
    GameObject player;
    RaycastHit2D rayHitWall, rayHitPlatform;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    SpriteRenderer spriteLever;
    bool isLeft;
    float sizeX, sizeY;
    bool isRotating;
    public bool isDestroyed;
    public float speed;

    void Awake() {
        player = GameObject.FindWithTag("Player");
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        BoxCollider2D collid = GetComponent<BoxCollider2D>();
        sizeX = collid.size.x;
        sizeY = collid.size.y;
    }

    void Start() {
        StartCoroutine(Moving());
        //AutoChangeDir();
    }

    IEnumerator Moving() {
        yield return new WaitForSeconds(0.5f);
        rigid.bodyType = RigidbodyType2D.Kinematic;
        while (!GameManager.instance.isRotating && !isDestroyed) {
            // Move
            transform.position = transform.position + transform.right * (isLeft ? -speed : speed) * Time.deltaTime;
            
            // Change direction
            if (isLeft) {
                rayHitWall = Physics2D.Raycast(transform.position - transform.right * sizeX * 0.6f, -transform.right, 0.1f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitPlatform = Physics2D.Raycast(transform.position - transform.right * sizeX * 0.5f, -transform.up, 1f, ~(1 << 18));
            }
            else {
                rayHitWall = Physics2D.Raycast(transform.position + transform.right * sizeX * 0.6f, transform.right, 0.1f, 1 << 3 | 1 << 10 | 1 << 18 | 1 << 19);
                rayHitPlatform = Physics2D.Raycast(transform.position + transform.right * sizeX * 0.5f, -transform.up, 1f, ~(1 << 18));
            }
            if (rayHitWall.collider != null || rayHitPlatform.collider == null) {
                isLeft = !isLeft;
            }
            yield return null;
        }
        if (isDestroyed) {
            StartCoroutine(Destroyed());
        }
        else {
            StartCoroutine(Rotating());
        }
    }

    IEnumerator Rotating() {
        while (GameManager.instance.isRotating) {
            transform.rotation = player.transform.rotation;
            yield return null;
        }
        StartCoroutine(Falling());
    }

    IEnumerator Falling() {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        while (Physics2D.Raycast(transform.position, -transform.up, sizeY, 1 << 3 | 1 << 19).collider == null) {
            yield return null;
        }
        StartCoroutine(Moving());
    }

    IEnumerator Destroyed() {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0.5f);
        if (transform.childCount != 0) {
            spriteLever = transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
            spriteLever.color = new Color(spriteLever.color.r, spriteLever.color.g, spriteLever.color.b, 0.5f);
        }
        Invoke("Erase", 1f);
        while (true) {
            Color color = sprite.color;
            color.a = 0.5f - color.a;
            sprite.color = color;
            if (transform.childCount != 0) {
                Color colorLever = spriteLever.color;
                colorLever.a = 0.5f - colorLever.a;
                spriteLever.color = colorLever;
            }
            yield return new WaitForSeconds(0.02f);
        }
    }

    void Erase() {
        if (transform.childCount != 0) {
            Transform lever = transform.GetChild(1);
            lever.parent = null;
            lever.gameObject.SetActive(true); 
        }
        Destroy(gameObject);
    }

    void AutoChangeDir() {
        isLeft = !isLeft;
        Invoke("AutoChangeDir", Random.Range(3, 10));
    }
}
