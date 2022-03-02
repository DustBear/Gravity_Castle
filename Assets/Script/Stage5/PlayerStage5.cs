using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStage5 : Player 
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Spike")
        {
            GameManager.instance.InitShakedFloorInfo();
            GameManager.instance.isDie = true;
            UIManager.instance.FadeOut();
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        if (other.tag == "Spike")
        {
            GameManager.instance.InitShakedFloorInfo();
            GameManager.instance.isDie = true;
            UIManager.instance.FadeOut();
        }
    }

    protected override void OnTriggerExit2D(Collider2D other) {
        base.OnTriggerExit2D(other);
    }

    protected override void Rope()
    {
        switch (ropingState)
        {
            case RopingState.idle:
                if (InputManager.instance.vertical == 1 && isCollideRope || shouldRope)
                {
                    rigid.gravityScale = 0;
                    rigid.velocity = Vector2.zero;
                    if (rope == null)
                    {
                        break;
                    }
                    ropingState = RopingState.access;
                }
                break;

            case RopingState.access:
                if (rope.CompareTag("VerticalRope"))
                {
                    destPos = new Vector2(rope.transform.position.x - transform.up.x * 0.7f, transform.position.y);
                }
                else
                {
                    destPos = new Vector2(transform.position.x, rope.transform.position.y - transform.up.y * 0.7f);
                }
                transform.position = Vector2.MoveTowards(transform.position, destPos, Time.deltaTime * 8f);
                if (Vector2.Distance(transform.position, destPos) < 0.1f)
                {
                    transform.position = destPos;
                    transform.parent = rope.transform;
                    isJumping = false;
                    ropingState = RopingState.move;
                }
                break;

            case RopingState.move:
                if (isJumping)
                {
                    transform.parent = null;
                    rigid.gravityScale = 2;
                    ropingState = RopingState.idle;
                }
                else
                {
                    shouldRope = false;
                    if (Physics2D.gravity.normalized == Vector2.left)
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            rigid.velocity = new Vector2(0, -InputManager.instance.horizontal * ropeSpeed);
                        }
                        else
                        {
                            rigid.velocity = new Vector2(InputManager.instance.vertical * ropeSpeed, 0);
                        }
                    }
                    else if (Physics2D.gravity.normalized == Vector2.right)
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            rigid.velocity = new Vector2(0, InputManager.instance.horizontal * ropeSpeed);
                        }
                        else
                        {
                            rigid.velocity = new Vector2(-InputManager.instance.vertical * ropeSpeed, 0);
                        }
                    }
                    else if (Physics2D.gravity.normalized == Vector2.up)
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            rigid.velocity = new Vector2(0, -InputManager.instance.vertical * ropeSpeed);
                        }
                        else
                        {
                            rigid.velocity = new Vector2(-InputManager.instance.horizontal * ropeSpeed, 0);
                        }
                    }
                    else
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            rigid.velocity = new Vector2(0, InputManager.instance.vertical * ropeSpeed);
                        }
                        else
                        {
                            rigid.velocity = new Vector2(InputManager.instance.horizontal * ropeSpeed, 0);
                        }
                    }
                }
                break;
        }
    }

    protected override void Lever()
    {
        switch (leveringState)
        {
            case LeveringState.idle:
                if (isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown
                    && lever.transform.eulerAngles.z == transform.eulerAngles.z)
                {
                    rigid.velocity = Vector2.zero;
                    animator.SetBool("isWalking", true);
                    switch (lever.transform.eulerAngles.z)
                    {
                        case 0f:
                            if (transform.localPosition.x > lever.transform.localPosition.x)
                            {
                                sprite.flipX = true;
                            }
                            else
                            {
                                sprite.flipX = false;
                            }
                            leveringState = LeveringState.moveToLever1;
                            break;

                        case 180f:
                            if (transform.localPosition.x > lever.transform.localPosition.x)
                            {
                                sprite.flipX = false;
                            }
                            else
                            {
                                sprite.flipX = true;
                            }
                            leveringState = LeveringState.moveToLever1;
                            break;
                        
                        case 90f:
                            if (transform.localPosition.y > lever.transform.localPosition.y)
                            {
                                sprite.flipX = true;
                            }
                            else
                            {
                                sprite.flipX = false;
                            }
                            leveringState = LeveringState.moveToLever2;
                            break;

                        case 270f:
                            if (transform.localPosition.y > lever.transform.localPosition.y)
                            {
                                sprite.flipX = false;
                            }
                            else
                            {
                                sprite.flipX = true;
                            }
                            leveringState = LeveringState.moveToLever2;
                            break;
                    }
                }
                break;

            case LeveringState.moveToLever1:
                // Move
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(lever.transform.localPosition.x, transform.localPosition.y), 3f * Time.deltaTime);

                // Finish moving
                if (transform.localPosition.x == lever.transform.localPosition.x)
                {
                    FinishMovingToLever();
                }
                break;

            case LeveringState.moveToLever2:
                // Move
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(transform.localPosition.x, lever.transform.localPosition.y), 3f * Time.deltaTime);

                // Finish moving
                if (transform.localPosition.y == lever.transform.localPosition.y)
                {
                    FinishMovingToLever();
                }
                break;

            case LeveringState.selectGravityDir:
                if (InputManager.instance.vertical == 1 && InputManager.instance.verticalDown || InputManager.instance.horizontalDown)
                {
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(false);

                    if (InputManager.instance.vertical == 1 && InputManager.instance.verticalDown)
                    {
                        leveringState = LeveringState.idle;
                    }
                    else
                    {
                        rigid.gravityScale = 0f;
                        rigid.velocity = Vector2.zero;
                        transform.parent = null; // For moving floor
                        destRotZ = transform.eulerAngles.z + InputManager.instance.horizontal * 90f;
                        if (destRotZ == 360f)
                        {
                            destRot = Quaternion.Euler(Vector3.zero);
                        }
                        else
                        {
                            if (destRotZ == -90f)
                            {
                                destRotZ = 270f;
                            }
                            destRot = Quaternion.Euler(Vector3.forward * destRotZ);
                        }
                        GameManager.instance.isChangeGravityDir = true;
                        if (lever.transform.eulerAngles.z == 0f || lever.transform.eulerAngles.z == 180f)
                        {
                            leveringState = LeveringState.changeGravityDir1;
                        }
                        else
                        {
                            leveringState = LeveringState.changeGravityDir2;
                        }
                    }
                }
                break;

            case LeveringState.changeGravityDir1:
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(transform.localPosition.x, lever.transform.position.y), 3f * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, destRot, Time.deltaTime * 8.0f);
                if (Mathf.RoundToInt(transform.eulerAngles.z) == destRotZ)
                {
                    FinishRotating();
                }
                break;

            case LeveringState.changeGravityDir2:
                transform.localPosition = Vector2.MoveTowards(transform.localPosition, new Vector2(lever.transform.position.x, transform.localPosition.y), 3f * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, destRot, Time.deltaTime * 8.0f);
                if (Mathf.RoundToInt(transform.eulerAngles.z) == destRotZ)
                {
                    FinishRotating();
                }
                break;

            case LeveringState.fall:
                if (isGrounded || Vector2.Dot(rigid.velocity, Physics2D.gravity) < 0f)
                {
                    leveringState = LeveringState.idle;
                }
                break;
        }
    }

    protected override void IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 3);

        // MovingPlatform
        RaycastHit2D rayHitMovingPlatform = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 16);
        if (rayHitMovingPlatform.collider != null && !GameManager.instance.isChangeGravityDir)
        {
            transform.parent = rayHitMovingPlatform.collider.transform;
        }
        else if (ropingState == RopingState.idle)
        {
            transform.parent = null;
        }
        isGrounded = rayHit.collider != null || rayHitMovingPlatform.collider != null;
    }
}
