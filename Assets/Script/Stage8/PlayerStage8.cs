using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage8 : Player
{
    [HideInInspector] public bool isDevilRotating;
    [HideInInspector] public bool isBlackHole;
    BoxCollider2D collide;
    bool isDevilFalling;
    bool isBlackHoleFalling;

    protected override void Awake()
    {
        base.Awake();
        collide = GetComponent<BoxCollider2D>();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (!GameManager.instance.isDie)
        {
            if (!isDevilRotating && !isBlackHole)
            {
                if (!isJumping && ropingState == RopingState.idle)
                {
                    Lever();
                }
                if (leveringState == LeveringState.idle)
                {
                    Rope();
                    if (ropingState != RopingState.access)
                    {
                        Jump();
                    }
                    if (ropingState == RopingState.idle)
                    {
                        Walk();
                    }
                }
            }
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        base.OnCollisionEnter2D(other);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        if (other.CompareTag("Devil")) {
            GameManager.instance.isDie = true;
            UIManager.instance.FadeOut();
        }
    }

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
    }

    protected override bool IsGrounded()
    {
       return base.IsGrounded();
    }

    public IEnumerator DevilUsingLever(float targetRot)
    {
        isDevilFalling = false;
        while (isBlackHole && !isBlackHoleFalling)
        {
            yield return null;
        }
        rigid.gravityScale = 0;
        rigid.velocity = Vector2.zero;

        // Rotate
        while (Mathf.Abs(transform.eulerAngles.z - targetRot) > 0.1f)
        {
            if (!isBlackHole ||isBlackHoleFalling)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, targetRot), Time.deltaTime * 8.0f);
            }
            yield return null;
        }

        // Set gravity
        Vector2 gravity = -transform.up * 9.8f;
        if (Mathf.Abs(gravity.x) < 1f)
        {
            gravity.x = 0f;
            Physics2D.gravity = gravity;
        }
        else
        {
            gravity.y = 0f;
            Physics2D.gravity = gravity;
        }

        // Fall
        isDevilFalling = true;
        rigid.gravityScale = 2;
        while (!IsGrounded() && !isBlackHole)
        {
            yield return null;
        }
        isDevilRotating = false;
    }

    public IEnumerator MoveToBlackHole(Vector2 startPos, Vector2 targetPos)
    {
        isBlackHoleFalling = false;
        // Move to the starting postion
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;
        collide.enabled = false;
        while ((Vector2)transform.position != startPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPos, 8f * Time.deltaTime);
            yield return null;
        }
        while (sprite.color.a > 0f)
        {
            Color color = sprite.color;
            color.a -= 0.1f;
            sprite.color = color;
            yield return new WaitForSeconds(0.05f);
        }

        // Move to the target position
        while ((Vector2)transform.position != targetPos)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 30f * Time.deltaTime);
            yield return null;
        }
        while (sprite.color.a < 1f)
        {
            Color color = sprite.color;
            color.a += 0.1f;
            sprite.color = color;
            yield return new WaitForSeconds(0.1f);
        }

        // Fall
        isBlackHoleFalling = true;
        collide.enabled = true;
        if (!isDevilRotating || isDevilFalling)
        {
            rigid.gravityScale = 2f;
        }
        while (!IsGrounded())
        {
            yield return null;
        }
        isBlackHole = false;
    }
}
