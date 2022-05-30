using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorShaking : MonoBehaviour
{
    [SerializeField] int floorNum;
    [SerializeField] float shakeRange;
    [SerializeField] float shakeDuration;
    [SerializeField] float waitingTime;
    [SerializeField] MainCamera mainCamera;
    protected Rigidbody2D rigid;
    protected SpriteRenderer render;
    protected enum State {idle, waiting, shaked, falling, fading};
    protected State state;
    protected Vector2 nextPos;
    protected float time;
    protected float shakeDegreeX;
    protected float shakeDegreeY;

    bool isGravityDirChanged;
    Vector2 storedVel;

    virtual protected void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        nextPos = transform.position;
        state = State.idle;
        render = GetComponent<SpriteRenderer>();
    }

    virtual protected void Start()
    {
        if (GameManager.instance.curIsShaked[floorNum])
        {
            Destroy(gameObject);
        }
    }

    virtual protected void Update() {
        switch (state) {
            case State.idle:
                Idle();
                break;
            case State.waiting:
                Waiting();
                break;
            case State.shaked:
                Shaked();
                break;
            case State.falling:
                Falling();
                break;
            case State.fading:
                break;
        }
    }

    virtual protected void OnCollisionEnter2D(Collision2D other) {
        if (other.collider != null && other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.curIsShaked[floorNum] = true;
            state = State.waiting;
        }
    }

    virtual protected void Idle()
    {
        // RaycastHit2D rayHitPlayer;
        // if (Physics2D.gravity == new Vector2(-9.8f, 0f))
        // {
        //     rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x + transform.localScale.x / 2f + 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.7f), 0f, Vector2.right, 0.5f, 1 << 10);
        // }
        // else if (Physics2D.gravity == new Vector2(9.8f, 0f))
        // {
        //     rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x - transform.localScale.x / 2f - 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.7f), 0f, Vector2.left, 0.5f, 1 << 10);
        // }
        // else if (Physics2D.gravity == new Vector2(0f, 9.8f))
        // {
        //     rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2f - 0.1f), new Vector2(transform.localScale.x * 0.7f, 0.01f), 0f, Vector2.down, 0.5f, 1 << 10);
        // }
        // else
        // {
        //     rayHitPlayer = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2f + 0.1f), new Vector2(transform.localScale.x * 0.7f, 0.01f), 0f, Vector2.up, 0.5f, 1 << 10);
        // }
        // if (rayHitPlayer.collider != null)
        // {
        //     GameManager.instance.curIsShaked[floorNum] = true;
        //     state = State.waiting;
        // }
    }

    virtual protected void Waiting()
    {
        time += Time.deltaTime;
        if (time >= waitingTime)
        {
            time = 0;
            state = State.shaked;
        }
    }

    virtual protected void Shaked()
    {
        time += Time.deltaTime;
        if (Physics2D.gravity.x == 0)
        {
            if (shakeDegreeX == 0f)
            {
                shakeDegreeX = shakeRange;
            }
            else
            {
                shakeDegreeX *= -1;
            }
            transform.position = nextPos + Vector2.right * shakeDegreeX;
        }
        else
        {
            if (shakeDegreeY == 0f)
            {
                shakeDegreeY = shakeRange;
            }
            else
            {
                shakeDegreeY *= -1;
            }
            transform.position = nextPos + Vector2.up * shakeDegreeY;
        }

        if (time >= shakeDuration)
        {
            transform.position = nextPos;
            if (Physics2D.gravity.x == 0)
            {
                rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            }
            else
            {
                rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
            }
            rigid.bodyType = RigidbodyType2D.Dynamic;
            state = State.falling;
        }
    }

    virtual protected void Falling()
    {
        if (!isGravityDirChanged && Player.curState == Player.States.ChangeGravityDir)
        {
            rigid.gravityScale = 0f;
            storedVel = rigid.velocity;
            rigid.velocity = Vector2.zero;
            isGravityDirChanged = true;
        }
        else if (isGravityDirChanged && Player.curState != Player.States.ChangeGravityDir)
        {
            rigid.gravityScale = 2f;
            rigid.velocity = storedVel;
            isGravityDirChanged = false;
        }
        RaycastHit2D rayHitPlatform;
        if (Physics2D.gravity == new Vector2(-9.8f, 0f))
        {
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionX;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x - transform.localScale.x / 2f - 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.6f), 0f, Vector2.left, 0.5f, 1 << 3 | 1 << 16);
        }
        else if (Physics2D.gravity == new Vector2(9.8f, 0f))
        {
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionX;
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x + transform.localScale.x / 2f + 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.6f), 0f, Vector2.right, 0.5f, 1 << 3 | 1 << 16);
        }
        else if (Physics2D.gravity == new Vector2(0f, 9.8f))
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2f + 0.1f), new Vector2(transform.localScale.x * 0.6f, 0.01f), 0f, Vector2.up, 0.5f, 1 << 3 | 1 << 16);
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2f - 0.1f), new Vector2(transform.localScale.x * 0.6f, 0.01f), 0f, Vector2.down, 0.5f, 1 << 3 | 1 << 16);
        }
        if (rayHitPlatform.collider != null)
        {
            StartCoroutine(FadeOut());
            state = State.fading;
        }
    }

    protected IEnumerator FadeOut()
    {
        for (float f = 1.0f; f >= 0.0f; f -= 0.1f)
        {
            Color color = render.material.color;
            color.a = f;
            render.material.color = color;
            yield return new WaitForSeconds(0.02f);
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform trans = transform.GetChild(i);
            if (trans.CompareTag("Player"))
            {
                trans.parent = null;
            }
            else if (trans.CompareTag("Lever"))
            {
                SpriteRenderer leverRenderer = trans.GetComponent<SpriteRenderer>();
                BoxCollider2D leverCollider = trans.GetComponent<BoxCollider2D>();
                leverRenderer.enabled = false;
                leverCollider.enabled = false;
                trans.parent = null;
            }
        }
        Destroy(gameObject);
    }
}
