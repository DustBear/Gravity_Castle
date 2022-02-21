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

    protected override void Lever()
    {
        switch (leveringState)
        {
            case LeveringState.idle:
                if (isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown)
                {
                    rigid.velocity = Vector2.zero;
                    rigid.constraints = RigidbodyConstraints2D.FreezePosition;
                    animator.SetBool("isWalking", false);
                    leftArrow.SetActive(true);
                    rightArrow.SetActive(true);
                    leveringState = LeveringState.selectGravityDir;
                }
                break;

            case LeveringState.selectGravityDir:
                if (InputManager.instance.vertical == 1 && InputManager.instance.verticalDown || InputManager.instance.horizontalDown || !isCollideLever)
                {
                    rigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(false);
                    transform.parent = null; // For moving floor

                    if (InputManager.instance.vertical == 1 && InputManager.instance.verticalDown || !isCollideLever)
                    {
                        leveringState = LeveringState.idle;
                    }
                    else
                    {
                        float destRotZ = transform.eulerAngles.z + InputManager.instance.horizontal * 90f;
                        if (destRotZ > 180f)
                        {
                            destRotZ -= 360f;
                        }
                        destRot = Vector3.forward * destRotZ;
                        leveringState = LeveringState.changeGravityDir;
                    }
                }
                break;

            case LeveringState.changeGravityDir:
                GameManager.instance.isChangeGravityDir = true; 
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(destRot), Time.deltaTime * 8.0f);
                float tmp = Mathf.Abs(transform.eulerAngles.z - destRot.z);
                // If finish rotating
                if (Mathf.Abs(tmp - 360f) < 0.1f || tmp < 0.1f)
                {
                    GameManager.instance.isChangeGravityDir = false;
                    transform.eulerAngles = destRot;
                    Vector2 gravity = -transform.up * 9.8f;
                    if (Mathf.Abs(gravity.x) < 1f)
                    {
                        gravity.x = 0f;
                    }
                    else
                    {
                        gravity.y = 0f;
                    }
                    Physics2D.gravity = gravity;
                    leveringState = LeveringState.fall;
                }
                break;

            case LeveringState.fall:
                if (IsGrounded())
                {
                    leveringState = LeveringState.idle;
                }
                break;
        }
    }

    protected override bool IsGrounded()
    {
       return base.IsGrounded();
    }

    public IEnumerator DevilUsingLever(float targetRot)
    {
        isDevilRotating = true;
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
            transform.position = Vector2.MoveTowards(transform.position, targetPos, 50f * Time.deltaTime);
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
