using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] protected GameObject leftArrow;
    [SerializeField] protected GameObject rightArrow;
    [SerializeField] protected float defaultGravityScale = 3f;
    [SerializeField] protected float maxVelocity = 20f;
    protected Rigidbody2D rigid;
    protected SpriteRenderer sprite;
    protected bool isGrounded;
    public Animator ani;

    // Collision
    protected bool isCollideRope;
    protected bool isCollideLever;

    // Walk
    [SerializeField] protected float walkSpeed;

    // Jump
    [SerializeField] protected float jumpPower;
    [HideInInspector] public bool isJumping;
    protected bool isFalling;

    // Rope
    [HideInInspector] public enum RopingState { idle, access, move };
    [HideInInspector] public RopingState ropingState;
    protected Vector3 destPos;
    protected GameObject rope;
    [SerializeField] public float ropeSpeed;
    protected bool shouldRope;

    // Lever
    [HideInInspector] public enum LeveringState { idle, moveToLever1, moveToLever2, selectGravityDir, changeGravityDir1, changeGravityDir2, fall };
    [HideInInspector] public LeveringState leveringState;
    protected Quaternion destRot;
    protected float destRotZ;
    protected GameObject lever;

    virtual protected void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }

    virtual protected void Start()
    {
        UIManager.instance.FadeIn();
        GameManager.instance.isChangeGravityDir = false;
        // Go to Next Scene
        if (!GameManager.instance.shouldStartAtSavePoint)
        {
            transform.position = GameManager.instance.nextPos;
            Physics2D.gravity = GameManager.instance.nextGravityDir * 9.8f;
            transform.up = -GameManager.instance.nextGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z;
            leveringState = GameManager.instance.nextLeveringState;
            isJumping = GameManager.instance.nextIsJumping;

            if (GameManager.instance.nextRopingState != RopingState.idle)
            {
                rigid.gravityScale = 0f;
                shouldRope = true;
            }
        }
        else
        {
            transform.position = GameManager.instance.gameData.respawnPos;
            Physics2D.gravity = GameManager.instance.gameData.respawnGravityDir * 9.8f;
            transform.up = -GameManager.instance.gameData.respawnGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z;
            GameManager.instance.shouldStartAtSavePoint = false;
        }
    }

    virtual protected void Update()
    {
        if (GameManager.instance.shouldStartAtSavePoint) return;

        // 최대 속도 제한
        rigid.velocity = Vector2.ClampMagnitude(rigid.velocity, maxVelocity);

        // Animation
        IsGrounded();
        ani.SetBool("isFloating", !isGrounded);

        AnimatorStateInfo aniState = ani.GetCurrentAnimatorStateInfo(0);
        if (aniState.normalizedTime >= 1f)
        {
            if (aniState.IsName("Jump"))
            {
                ani.SetBool("isJumping", false);
            }
            else if (isGrounded && aniState.IsName("Float")
            && (leveringState != LeveringState.changeGravityDir1 || leveringState != LeveringState.changeGravityDir2))
            {
                ani.SetBool("isLanding", true);
            }
            else if (aniState.IsName("Land"))
            {
                ani.SetBool("isLanding", false);
            }
        }

        // Walk, Jump, Rope, Lever
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

    virtual protected void OnCollisionEnter2D(Collision2D other)
    {
        // Spike와 부딫히면 사망
        if (other.gameObject.CompareTag("Spike"))
        {
            GameManager.instance.shouldStartAtSavePoint = true;
            ropingState = RopingState.idle;
            leveringState = LeveringState.idle;
            rigid.gravityScale = defaultGravityScale;
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
            lever = other.gameObject;
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
             ani.SetBool("isWalking", true);
        }
         else
        {
             ani.SetBool("isWalking", false);
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
        if (InputManager.instance.jump && !isJumping && (isGrounded || ropingState == RopingState.move))
        {
            rigid.velocity = Vector2.zero;
            if (InputManager.instance.vertical == -1)
            {
                rigid.AddForce(-transform.up * jumpPower, ForceMode2D.Impulse);
            }
            else
            {
                rigid.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
                ani.SetBool("isJumping", true);
            }
            isJumping = true;
            ani.SetBool("isRopingVerticalIdle", false);
            ani.SetBool("isRopingHorizontalIdle", false);
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
            else if (isGrounded)
            {
                isJumping = false;
                isFalling = false;
            }
        }
    }

    protected virtual void Rope()
    {
        switch (ropingState)
        {
            case RopingState.idle:
                // rope에 매달리려는 시도가 있거나
                // 이전 씬에서 로프에 접근한 상태로 현재 씬으로 넘어온 경우(shouldRope)
                if (InputManager.instance.vertical == 1 && isCollideRope || shouldRope)
                {
                    rigid.gravityScale = 0f;
                    rigid.velocity = Vector2.zero;
                    if (rope == null)
                    {
                        break;
                    }
                    // 매달려야 하는 위치 설정(destPos)
                    Vector2 ropePos = rope.transform.position;
                    if (rope.CompareTag("VerticalRope"))
                    {
                        destPos = new Vector2(ropePos.x, transform.position.y);
                    }
                    else
                    {
                        destPos = new Vector2(transform.position.x, ropePos.y);
                    }

                    ropingState = RopingState.access;
                }
                break;

            case RopingState.access:
                transform.position = Vector2.MoveTowards(transform.position, destPos, Time.deltaTime * 8f);
                // 플레이어가 rope에 근접했을 때
                if (Vector2.Distance(transform.position, destPos) < 0.1f)
                {
                    // 플레이어를 rope로 완전히 이동시킴
                    transform.position = destPos;
                    transform.parent = rope.transform;
                    isJumping = false;

                    // float 애니메이션 멈추고 rope에 매달리는 애니메이션 실행
                    ani.SetBool("isFloating", false);
                    if (Physics2D.gravity.normalized.y == 0f)
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            ani.SetBool("isRopingHorizontalIdle", true);
                        }
                        else
                        {
                            ani.SetBool("isRopingVerticalIdle", true);
                        }
                    }
                    else
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            ani.SetBool("isRopingVerticalIdle", true);
                        }
                        else
                        {
                            ani.SetBool("isRopingHorizontalIdle", true);
                        }
                    }
                    
                    ropingState = RopingState.move;
                }
                break;

            case RopingState.move:
                // Rope에서 떨어질 때
                if (!isCollideRope || isJumping)
                {
                    transform.parent = null;
                    rigid.gravityScale = defaultGravityScale;
                    ropingState = RopingState.idle;
                }
                // Rope에 매달린 상태. 이동 가능
                else
                {
                    shouldRope = false;
                    if (Physics2D.gravity.normalized == Vector2.left)
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            HorizontalRopeAni();
                            rigid.velocity = new Vector2(0, -InputManager.instance.horizontal * ropeSpeed);
                        }
                        else
                        {
                            VerticalRopeAni();
                            rigid.velocity = new Vector2(InputManager.instance.vertical * ropeSpeed, 0);
                        }
                    }
                    else if (Physics2D.gravity.normalized == Vector2.right)
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            HorizontalRopeAni();
                            rigid.velocity = new Vector2(0, InputManager.instance.horizontal * ropeSpeed);
                        }
                        else
                        {
                            VerticalRopeAni();
                            rigid.velocity = new Vector2(-InputManager.instance.vertical * ropeSpeed, 0);
                        }
                    }
                    else if (Physics2D.gravity.normalized == Vector2.up)
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            VerticalRopeAni();
                            rigid.velocity = new Vector2(0, -InputManager.instance.vertical * ropeSpeed);
                        }
                        else
                        {
                            HorizontalRopeAni();
                            rigid.velocity = new Vector2(-InputManager.instance.horizontal * ropeSpeed, 0);
                        }
                    }
                    else
                    {
                        if (rope.CompareTag("VerticalRope"))
                        {
                            VerticalRopeAni();
                            rigid.velocity = new Vector2(0, InputManager.instance.vertical * ropeSpeed);
                        }
                        else
                        {
                            HorizontalRopeAni();
                            rigid.velocity = new Vector2(InputManager.instance.horizontal * ropeSpeed, 0);
                        }
                    }
                }
                break;
        }
    }

    protected virtual void Lever()
    {
        switch (leveringState)
        {
            case LeveringState.idle:
                if (isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown
                    && lever.transform.eulerAngles.z == transform.eulerAngles.z)
                {
                    rigid.velocity = Vector2.zero;
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
                if (isGrounded)
                {
                    leveringState = LeveringState.idle;
                }
                break;
        }
    }

    protected virtual void FinishMovingToLever()
    {
        // animator.SetBool("isWalking", false);
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
        leveringState = LeveringState.selectGravityDir;
    }

    protected virtual void FinishRotating()
    {
        if (destRotZ == 360f)
        {
            destRotZ = 0f;
        }
        transform.eulerAngles = Vector3.forward * destRotZ;
        GameManager.instance.isChangeGravityDir = false;
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
        rigid.gravityScale = 3f;
        leveringState = LeveringState.fall;
    }

    protected virtual void IsGrounded()
    {
        RaycastHit2D rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 16);
        isGrounded = rayHit.collider != null;
    }

    protected virtual void VerticalRopeAni()
    {
        if (InputManager.instance.vertical == 0)
        {
            ani.SetBool("isRopingVerticalMove", false);
        }
        else
        {
            ani.SetBool("isRopingVerticalMove", true);
        }
    }

    protected virtual void HorizontalRopeAni()
    {
        if (InputManager.instance.horizontal == 0)
        {
            ani.SetBool("isRopingHorizontalMove", false);
        }
        else
        {
            ani.SetBool("isRopingHorizontalMove", true);
        }
    }
}