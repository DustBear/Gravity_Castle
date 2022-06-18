using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MonsterLove.StateMachine;

public class FloorShaking : MonoBehaviour
{
    [SerializeField] int floorNum;
    [SerializeField] float shakeRange;
    [SerializeField] float shakeDuration;
    [SerializeField] float waitingTime;

    Rigidbody2D rigid;
    BoxCollider2D collide;
    SpriteRenderer render;
    float time;
    Vector2 shakeDegree;
    float xScale, yScale;
    

    enum States {Idle, Wait, Shake, Fall, FadeOut}
    StateMachine<States> fsm;

    protected Vector2 nextPos;

    virtual protected void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        collide = GetComponent<BoxCollider2D>();
        render = GetComponent<SpriteRenderer>();
        nextPos = transform.position;
        fsm = StateMachine<States>.Initialize(this);
        if (transform.eulerAngles.z == 0f || transform.eulerAngles.z == 180f)
        {
            xScale = collide.size.x;
            yScale = collide.size.y;
        }
        else
        {
            xScale = collide.size.y;
            yScale = collide.size.x;
        }
    }

    virtual protected void Start()
    {
        // 플랫폼이 사라진 상태로 게임이 저장되었다면 사라진 상태로 그대로 놔둠
        if (GameManager.instance.curIsShaked[floorNum])
        {
            Destroy(gameObject);
        }
        fsm.ChangeState(States.Idle);
    }

    virtual protected void Idle_Update()
    {
        // 플레이어가 밟으면 떨어질 준비
        RaycastHit2D rayHitPlatform;
        if (Physics2D.gravity == Vector2.right * 9.8f)
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x - (xScale / 2f + 0.1f), transform.position.y), new Vector2(0.01f, yScale * 0.6f), 0f, Vector2.left, 0.5f, 1 << 10);
        }
        else if (Physics2D.gravity == Vector2.left * 9.8f)
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x + xScale / 2f + 0.1f, transform.position.y), new Vector2(0.01f, yScale * 0.6f), 0f, Vector2.right, 0.5f, 1 << 10);
        }
        else if (Physics2D.gravity == Vector2.down * 9.8f)
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + yScale / 2f + 0.1f), new Vector2(xScale * 0.6f, 0.01f), 0f, Vector2.up, 0.5f, 1 << 10);
        }
        else
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - (yScale / 2f + 0.1f)), new Vector2(xScale * 0.6f, 0.01f), 0f, Vector2.down, 0.5f, 1 << 10);
        }

        if (rayHitPlatform.collider != null)
        {
            GameManager.instance.curIsShaked[floorNum] = true;
            fsm.ChangeState(States.Wait);
        }
    }

    virtual protected void Wait_Update()
    {
        // 지정된 시간이 경과하면 흔들리기 위해 State 전환
        time += Time.deltaTime;
        if (time >= waitingTime)
        {
            fsm.ChangeState(States.Shake);
        }
    }

    protected void Shake_Enter()
    {
        time = 0f;

        // 얼마나 흔들릴지 설정
        if (Physics2D.gravity.x == 0)
        {
            shakeDegree = Vector2.right * shakeRange;
        }
        else
        {
            shakeDegree = Vector2.up * shakeRange;
        }
    }

    virtual protected void Shake_Update()
    {
        // 흔들림
        shakeDegree *= -1;
        transform.position = nextPos + shakeDegree;

        // 지정된 시간이 경과했으면 떨어지기 위해 State 전환
        time += Time.deltaTime;
        if (time >= shakeDuration)
        {
            fsm.ChangeState(States.Fall);
        }
    }

    protected void Fall_Enter()
    {
        // 원래 위치로 이동
        transform.position = nextPos;

        // 중력 영향 받고 떨어지기 시작
        rigid.bodyType = RigidbodyType2D.Dynamic;

        // 떨어지는 동안 외부의 힘에 의해 옆으로 밀리거나 회전하면 안됨
        if (Physics2D.gravity.x == 0)
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
        }
        else
        {
            rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
        }
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    virtual protected void Fall_Update()
    {
        if (Player.curState == Player.States.ChangeGravityDir)
        {
            // 중력방향이 전환될 때는 시간이 멈추어야 함
            if (rigid.gravityScale != 0f)
            {
                rigid.gravityScale = 0f;
                rigid.velocity = Vector2.zero;
            }
        }

        // 중력방향 전환이 끝났다면
        else if (rigid.gravityScale == 0f)
        {
            // 중력 방향이 90도 바뀌었으므로 x_pos freeze였다면 y_pos freeze로, y_pos freeze였다면 x_pos freeze로 바꿈
            if (Physics2D.gravity.x == 0)
            {
                rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
                rigid.constraints = ~RigidbodyConstraints2D.FreezePositionY;
            }
            else
            {
                rigid.constraints = RigidbodyConstraints2D.FreezePositionY;
                rigid.constraints = ~RigidbodyConstraints2D.FreezePositionX;
            }

            rigid.gravityScale = 2f;
        }

        // 바닥면이 플랫폼, 가시, 플레이어에게 부딫히면 사라짐
        RaycastHit2D rayHitPlatform;
        if (Physics2D.gravity == Vector2.left * 9.8f)
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x - (transform.localScale.x / 2f + 0.1f), transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.6f), 0f, Vector2.left, 0.5f, 1 << 3 | 1 << 10 | 1 << 16 | 1 << 19);
        }
        else if (Physics2D.gravity == Vector2.right * 9.8f)
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x + transform.localScale.x / 2f + 0.1f, transform.position.y), new Vector2(0.01f, transform.localScale.y * 0.6f), 0f, Vector2.right, 0.5f, 1 << 3 | 1 << 10 | 1 << 16 | 1 << 19);
        }
        else if (Physics2D.gravity == Vector2.up * 9.8f)
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y + transform.localScale.y / 2f + 0.1f), new Vector2(transform.localScale.x * 0.6f, 0.01f), 0f, Vector2.up, 0.5f, 1 << 3 | 1 << 10 | 1 << 16 | 1 << 19);
        }
        else
        {
            rayHitPlatform = Physics2D.BoxCast(new Vector2(transform.position.x, transform.position.y - (transform.localScale.y / 2f + 0.1f)), new Vector2(transform.localScale.x * 0.6f, 0.01f), 0f, Vector2.down, 0.5f, 1 << 3 | 1 << 10 | 1 << 16 | 1 << 19);
        }

        if (rayHitPlatform.collider != null)
        {
            fsm.ChangeState(States.FadeOut);
        }
    }

    protected IEnumerator FadeOut_Enter()
    {
        // Fade out
        var wait = new WaitForSeconds(0.1f);
        for (float f = 1f; f >= 0f; f -= 0.1f)
        {
            Color color = render.material.color;
            color.a = f;
            render.material.color = color;
            yield return wait;
        }

        // 플랫폼의 자식 오브젝트로 Player가 있다면 분리
        Transform player = transform.Find("Player");
        if (player != null) player.parent = null;

        Destroy(gameObject);
    }
}
