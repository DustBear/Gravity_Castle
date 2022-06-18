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
        // �÷����� ����� ���·� ������ ����Ǿ��ٸ� ����� ���·� �״�� ����
        if (GameManager.instance.curIsShaked[floorNum])
        {
            Destroy(gameObject);
        }
        fsm.ChangeState(States.Idle);
    }

    virtual protected void Idle_Update()
    {
        // �÷��̾ ������ ������ �غ�
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
        // ������ �ð��� ����ϸ� ��鸮�� ���� State ��ȯ
        time += Time.deltaTime;
        if (time >= waitingTime)
        {
            fsm.ChangeState(States.Shake);
        }
    }

    protected void Shake_Enter()
    {
        time = 0f;

        // �󸶳� ��鸱�� ����
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
        // ��鸲
        shakeDegree *= -1;
        transform.position = nextPos + shakeDegree;

        // ������ �ð��� ��������� �������� ���� State ��ȯ
        time += Time.deltaTime;
        if (time >= shakeDuration)
        {
            fsm.ChangeState(States.Fall);
        }
    }

    protected void Fall_Enter()
    {
        // ���� ��ġ�� �̵�
        transform.position = nextPos;

        // �߷� ���� �ް� �������� ����
        rigid.bodyType = RigidbodyType2D.Dynamic;

        // �������� ���� �ܺ��� ���� ���� ������ �и��ų� ȸ���ϸ� �ȵ�
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
            // �߷¹����� ��ȯ�� ���� �ð��� ���߾�� ��
            if (rigid.gravityScale != 0f)
            {
                rigid.gravityScale = 0f;
                rigid.velocity = Vector2.zero;
            }
        }

        // �߷¹��� ��ȯ�� �����ٸ�
        else if (rigid.gravityScale == 0f)
        {
            // �߷� ������ 90�� �ٲ�����Ƿ� x_pos freeze���ٸ� y_pos freeze��, y_pos freeze���ٸ� x_pos freeze�� �ٲ�
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

        // �ٴڸ��� �÷���, ����, �÷��̾�� �΋H���� �����
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

        // �÷����� �ڽ� ������Ʈ�� Player�� �ִٸ� �и�
        Transform player = transform.Find("Player");
        if (player != null) player.parent = null;

        Destroy(gameObject);
    }
}
