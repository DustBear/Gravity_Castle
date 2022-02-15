using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject rightArrow;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sprite;
    protected Animator animator;

    // Collision
    protected bool isCollideRope;
    protected bool isCollideLever;

    // Walk
    [SerializeField] protected float walkSpeed;

    // jump
    [SerializeField] float jumpPower;
    [HideInInspector] public bool isJumping;
    bool isFalling;

    // Rope
    [HideInInspector] public enum RopingState { idle, access, move };
    [HideInInspector] public RopingState ropingState;
    Vector3 destPos;
    GameObject rope;
    [SerializeField] public float ropeSpeed;
    bool shouldRope;

    // Lever
    [HideInInspector] public enum LeveringState { idle, selectGravityDir, changeGravityDir, fall };
    [HideInInspector] public LeveringState leveringState;
    Vector3 destRot;

    virtual protected void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    virtual protected void Start()
    {
        UIManager.instance.FadeIn();
        // Go to Next Scene
        if (!GameManager.instance.isDie)
        {
            transform.position = GameManager.instance.nextPos;
            Physics2D.gravity = GameManager.instance.nextGravityDir * 9.8f;
            transform.up = -GameManager.instance.nextGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z;
            leveringState = GameManager.instance.nextLeveringState;
            isJumping = GameManager.instance.nextIsJumping;
        }
        // Respawn after Dying
        else
        {
            transform.position = GameManager.instance.respawnPos;
            Physics2D.gravity = GameManager.instance.respawnGravityDir * 9.8f;
            transform.up = -GameManager.instance.respawnGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z;
            GameManager.instance.isDie = false;
        }

        if (GameManager.instance.nextRopingState != RopingState.idle)
        {
            rigid.gravityScale = 0f;
            shouldRope = true;
        }
    }

    virtual protected void Update()
    {
        if (!GameManager.instance.isDie)
        {
            if (!isJumping && ropingState == RopingState.idle)
            {
                Lever();
            }
            if (leveringState == LeveringState.idle)
            {
                Rope();
                if (!shouldRope)
                {
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

    virtual protected void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Spike")
        {
            GameManager.instance.isDie = true;
            UIManager.instance.FadeOut();
        }
    }

    virtual protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("VerticalRope") || other.CompareTag("HorizontalRope"))
        {
            isCollideRope = true;
            rope = other.gameObject;
        }
        if (other.CompareTag("Lever"))
        {
            isCollideLever = true;
        }
    }

    virtual protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VerticalRope") || other.CompareTag("HorizontalRope"))
        {
            isCollideRope = false;
        }
        if (other.CompareTag("Lever"))
        {
            isCollideLever = false;
        }
    }

    virtual protected void Walk()
    {
        // Move
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        locVel = new Vector2(InputManager.instance.horizontal * walkSpeed, locVel.y);
        rigid.velocity = transform.TransformDirection(locVel);

        // Animation
        if (InputManager.instance.horizontal != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
        if (InputManager.instance.horizontal == 1)
        {
            sprite.flipX = false;
        }
        else if (InputManager.instance.horizontal == -1)
        {
            sprite.flipX = true;
        }
    }

    protected void Jump()
    {
        // Start jumping
        if (InputManager.instance.jump && !isJumping)
        {
            rigid.velocity = Vector2.zero;
            if (InputManager.instance.vertical == -1)
            {
                rigid.AddForce(-transform.up * jumpPower, ForceMode2D.Impulse);
            }
            else
            {
                rigid.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
            }
            animator.SetBool("isJumping", true);
            isJumping = true;
        }

        // Landing on the ground -> Finish jumping
        if (isJumping)
        {
            if (!isFalling)
            {
                Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
                if (locVel.y <= 0f)
                {
                    isFalling = true;
                }
            }
            else if (IsGrounded())
            {
                animator.SetBool("isJumping", false);
                isJumping = false;
                isFalling = false;
            }
        }
    }

    protected void Rope()
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
                    Vector2 ropePos = rope.transform.position;
                    if (rope.CompareTag("VerticalRope"))
                    {
                        destPos = new Vector2(ropePos.x - transform.up.x * 0.7f, transform.position.y);
                    }
                    else
                    {
                        destPos = new Vector2(transform.position.x, ropePos.y - transform.up.y * 0.7f);
                    }
                    ropingState = RopingState.access;
                }
                break;

            case RopingState.access:
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
                if (!isCollideRope || isJumping)
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

    protected void Lever()
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
                if (InputManager.instance.vertical == 1 && InputManager.instance.verticalDown || InputManager.instance.horizontalDown)
                {
                    rigid.constraints = ~RigidbodyConstraints2D.FreezePosition;
                    leftArrow.SetActive(false);
                    rightArrow.SetActive(false);
                    transform.parent = null; // For moving floor

                    if (InputManager.instance.vertical == 1 && InputManager.instance.verticalDown)
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

    protected virtual bool IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.6f, 0.1f), transform.eulerAngles.z, -transform.up, 0.8f, 1 << 3 | 1 << 16);
        return rayHit.collider != null;
    }
}