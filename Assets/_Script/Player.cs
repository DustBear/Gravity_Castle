using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MonsterLove.StateMachine;

public class Player : MonoBehaviour
{
    [SerializeField] float defaultGravityScale;
    [SerializeField] float maxFallingSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float minJumpPower;
    [SerializeField] float maxJumpPower;
    [SerializeField] float maxJumpBoostPower;
    [SerializeField] float jumpChargeSpeed; // 점프 게이지 차는 속도
    [SerializeField] float ropeAccessSpeed; // rope에 접근하는 속도
    [SerializeField] float ropeMoveSpeed; // rope에 매달려 움직이는 속도
    [SerializeField] float leverMoveSpeed; // lever 작동 후 플레이어가 이동하는 속도
    [SerializeField] float leverRotateSpeed; // lever 작동 후 플레이어가 회전하는 속도
    [SerializeField] float windForce; // Stage3의 바람에 의해 받는 힘
    [SerializeField] float slidingDegree; // Stage6의 얼음 위에서 미끄러지는 정도

    // 플레이의 상태는 Finite State Machine으로 관리
    public enum States
    {
        Walk, Fall, Land, Jump, 
        PowerJump, // Stage5 강화 점프
        AccessRope, MoveOnRope,
        AccessLever, SelectGravityDir, ChangeGravityDir, FallAfterLevering,
        GhostUsingLever, FallAfterGhostLevering // Stage4 한정
    }
    StateMachine<States> fsm;
    [HideInInspector] public static States curState {get; private set;}
    Func<bool> readyToFall, readyToLand, readyToJump, readyToPowerJump, readyToRope, readyToLever; // 각 상태로 이동하기 위한 기본 조건

    // Jump
    [SerializeField]float jumpGauge;

    // Rope
    bool isCollideRope;
    GameObject rope; // 매달릴 rope

    // Lever
    bool isCollideLever;
    GameObject lever; // 작동시킬 lever
    Vector3 destPos_afterLevering; // Lever 작동 후 플레이어 position
    float destRot; // Lever 작동 후 플레이어 z-rotation

    // Wind
    bool isHorizontalWind;
    bool isVerticalWind;

    // 플레이어 아래에 있는 플랫폼 감지
    [SerializeField]bool isGrounded;
    [SerializeField] bool isOnJumpPlatform; //강화점프 발판 위에 있는지 감지
    RaycastHit2D rayHitIcePlatform;
    RaycastHit2D rayHitJumpBoost;

    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject rightArrow;
    MainCamera mainCamera;
    Rigidbody2D rigid;
    BoxCollider2D collide;
    SpriteRenderer sprite;
    Animator ani;

    //씬 시작할 때 엘리베이터와 상호작용
    GameObject openingSceneElevator;

    // Stage8 한정
    /********Stage8 기믹 수정할 때 코드 수정 필요 **********/
    [HideInInspector] public bool isDevilRotating;
    [HideInInspector] public bool isBlackHole;
    bool isDevilFalling;
    bool isBlackHoleFalling;

    //오디오 소스 
    AudioSource sound;
    [SerializeField] AudioClip moveSound; //걷기 or 로프를 탈 때 나는 소리
    [SerializeField] AudioClip jump_landSound; //점프 및 착지할 때 나는 소리 

    void Awake()
    {      
        fsm = StateMachine<States>.Initialize(this);
        rigid = GetComponent<Rigidbody2D>();
        collide = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<MainCamera>();
        openingSceneElevator = GameObject.Find("openingSceneElevator");
        sound = GetComponent<AudioSource>();
    }

    void Start()
    {
        UIManager.instance.FadeIn();

        // 각 State로 넘어가기 위한 기본 조건
        readyToFall = () => (!isGrounded)&&(!isOnJumpPlatform); //땅이나 점프강화발판 둘 다에 닿아있지 않을 때 
        readyToLand = () => (isGrounded || isOnJumpPlatform) && (int)transform.InverseTransformDirection(rigid.velocity).y <= 0; //땅or점프강화발판에 닿아 있고 y방향 속도벡터의 방향이 -1일 때 
        readyToJump = () => InputManager.instance.jumpDown && (!isOnJumpPlatform); //스페이스바를 눌렀고 강화점프 발판 위에 있지 않을 때 
        readyToPowerJump = () =>  InputManager.instance.jumpUp && (isOnJumpPlatform); //강화점프 발판 위에서 스페이스바를 눌렀다 뗄 때
        readyToRope = () => isCollideRope && InputManager.instance.vertical == 1; //위쪽 화살표 누르고 있고 + 로프에 닿아있을 때
        readyToLever = () => isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown 
                            && lever.transform.eulerAngles.z == transform.eulerAngles.z;
                            //레버에 닿아 있고, 레버와 동일한 rotation을 가지고 있고, 위쪽 화살표를 누르고 있을 때 

        // Scene이 세이브 포인트에서 시작하지 않을 때 플레이어 데이터 설정
        // 현재 Scene으로 넘어오기 직전의 데이터를 불러와서 적용
        if (!GameManager.instance.shouldStartAtSavePoint)
        {
            transform.position = GameManager.instance.nextPos;
            Physics2D.gravity = GameManager.instance.nextGravityDir * 9.8f;
            transform.up = -GameManager.instance.nextGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z; // x-rotation, y-rotation을 0으로 설정
            
            States nextState = GameManager.instance.nextState;
            // 이전 scene의 rope와 현재 scene의 rope는 다른 오브젝트로 취급되니 새로 접근 필요
            if (nextState == States.MoveOnRope) ChangeState(States.AccessRope);
            // Jump state로 시작하면 점프 input이 안들어와도 scene이 시작되자마자 플레이어가 점프하는 문제 발생
            else if (nextState == States.Jump) ChangeState(States.Fall);
            else ChangeState(nextState);
        }

        // Scene이 세이브 포인트에서 시작할 때 (사망한 후 부활, 저장된 게임 불러오기) 플레이어 데이터 설정
        else
        {
            transform.position = GameManager.instance.gameData.respawnPos;
            Physics2D.gravity = GameManager.instance.gameData.respawnGravityDir * 9.8f;
            transform.up = -GameManager.instance.gameData.respawnGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z; // x-rotation, y-rotation을 0으로 설정
            ChangeState(States.Walk); // 리스폰 시 플레이어가 미세하게 공중에 떠 있을 수 있으므로 Fall state로 시작
            GameManager.instance.shouldStartAtSavePoint = false;

            /*********** curAchievementNum 부분은 스테이지8 세이브 포인트 설정이 완료되면 수정 필요 ***********/
            if (GameManager.instance.gameData.curStageNum == 8 && GameManager.instance.gameData.curAchievementNum == 33 && !GameManager.instance.isCliffChecked)
            {
                InputManager.instance.isInputBlocked = true;
            }
        }

        //씬 시작할 때 플레이어의 위치를 scene Elevator에 넘겨줌(openingSceneElevator 를 작동할지 말지 결정)
        if(openingSceneElevator!= null)
        {
            openingSceneElevator.GetComponent<openingSceneEle>().playerPos = transform.position;
        }

        if (GameManager.instance.isStartWithFlipX)
        {
            sprite.flipX = true;
        }else
        {
            sprite.flipX = false;
        }
    }

    private void Update()
    {
        walkSoundCheck();
    }

    public void ChangeState(in States nextState)
    {
        fsm.ChangeState(nextState);
        curState = nextState;
    }

    void walkSoundCheck()
    {
        if(ani.GetBool("isWalking") && (isGrounded || isOnJumpPlatform))
        {
            if (!sound.isPlaying)
            {
                sound.clip = moveSound;
                sound.Play();
            }           
        }
        else
        {
            if(sound.clip == moveSound)
            {
                sound.Stop();
            }           
        }
    }

    void Walk_Enter() 
    {
        jumpGauge = minJumpPower;   
    }

    void Walk_Update() 
    {
        CheckGround();
        HorizontalMove();

        if (rayHitJumpBoost.collider != null)
        {
            ChargeJumpGauge();
        }
          
        if (readyToLever())
        {
            ChangeState(States.AccessLever);
        }
        else if (readyToRope())
        {
            ChangeState(States.AccessRope);
        }
        else if (readyToJump()) //스페이스바를 누르는 동안 작동
        {            
            ChangeState(States.Jump);
        }
        else if (readyToPowerJump())
        {          
            ChangeState(States.PowerJump);
        }          
        else if (readyToFall())
        {           
            ChangeState(States.Fall);
        }
    }

    public float defaultJumpSpeed;
    public float defaultJumpDelay; //일반 점프에서 최고높이에 도달하는 데 걸리는 시간
    public float maxJumpHeight; //일반 점프로 도달할 수 있는 최대 높이
    [SerializeField] float defaultJumpTimer;
    [SerializeField] bool shouldAddJumpForce = false; //true 인 동안 지속적으로 플레이어에 jumpForce를 가한다 

    float jumpUpdateForce(float timer) => -2 * (maxJumpHeight / defaultJumpDelay * defaultJumpDelay) * timer + 2 * (maxJumpHeight / defaultJumpDelay * defaultJumpDelay);
    //체공시간에 따른 플레이어 상승속도 계산 

    void ChargeJumpGauge() //스테이지5 강화점프 기믹에서만 사용 
    {
        if (InputManager.instance.jump) //스페이스바 누를 때 
        {
            // Jump boost 위에 있으면 점프력 증가 
            if (rayHitJumpBoost.collider != null)
            {
                jumpGauge = Mathf.Clamp(jumpGauge + jumpChargeSpeed * Time.deltaTime, minJumpPower, maxJumpBoostPower);
                Debug.Log("점프 게이지 모으는 중 : " + (jumpGauge - minJumpPower) / (maxJumpBoostPower - minJumpPower) * 100f + "%");
            }            
        }
    }

    void Jump_Enter()
    {
        rigid.velocity = Vector2.zero; // 바닥 플랫폼의 속도가 점프 속도에 영향을 미치는 것을 방지    
        StartCoroutine("jumpAddForce");
        
        defaultJumpTimer = 0; //타이머 0설정 
        shouldAddJumpForce = true;
        ani.SetBool("isJumping", true);

        sound.clip = jump_landSound;
        sound.Play();
    }

    IEnumerator jumpAddForce()
    {
        int jumpIndex = 0;

        while (jumpIndex < 50) //정밀한 체공을 위해 기존 Update문보다 짧은 동일 간격의 프레임으로 나눔
        {
            Vector2 addForceDirection;
            if (transform.up == new Vector3(0, 1, 0)) //플레이어가 위쪽을 향해 서 있을 때 
            {
                addForceDirection = new Vector2(rigid.velocity.x, defaultJumpSpeed);
            }
            else if (transform.up == new Vector3(0, -1, 0)) //플레이어가 아래쪽을 향해 서 있을 때 
            {
                addForceDirection = new Vector2(rigid.velocity.x, -defaultJumpSpeed);
            }
            else if (transform.up == new Vector3(1, 0, 0)) //플레이어가 오른쪽을 향해 서 있을 때 
            {
                addForceDirection = new Vector2(defaultJumpSpeed, rigid.velocity.y);
            }
            else //플레이어가 왼쪽을 향해 서 있을 때
            {
                addForceDirection = new Vector2(-defaultJumpSpeed, rigid.velocity.y);
            }

            rigid.velocity = addForceDirection;
            defaultJumpTimer += Time.deltaTime;

            //rigid.velocity = addForceDirection * defaultJumpSpeed * Time.deltaTime; 
            yield return new WaitForSeconds(defaultJumpDelay / 50);

            if (defaultJumpTimer >= defaultJumpDelay || InputManager.instance.jumpUp) //스페이스바를 떼거나 타이머가 끝나면 점프 중단
            {
                shouldAddJumpForce = false;
                defaultJumpTimer = 0; //타이머 0설정 
            }

            if (!shouldAddJumpForce)
            {
                break;
            }
        }
    }

    void Jump_Update()
    {
        LimitFallingSpeed();
        CheckGround();
        HorizontalMove();
   
        if (shouldAddJumpForce)
        {
            //rigid.velocity = new Vector2(rigid.velocity.x, jumpUpdateForce(defaultJumpTimer)); //스페이스바를 누르고 있는 동안 계속 힘 가함
        }
        
        /*
        if(defaultJumpTimer>=defaultJumpDelay || InputManager.instance.jumpUp) //스페이스바를 떼거나 타이머가 끝나면 점프 중단
        {
            shouldAddJumpForce = false;
            defaultJumpTimer = 0; //타이머 0설정 
        }        
        */

        if (readyToRope())
        {
            shouldAddJumpForce = false;
            ChangeState(States.AccessRope);
        }
        else if (readyToLand())
        {
            shouldAddJumpForce = false;
            ChangeState(States.Land);
        }
    }

    void PowerJump_Enter()
    {
        rigid.velocity = Vector2.zero; // 바닥 플랫폼의 속도가 점프 속도에 영향을 미치는 것을 방지
        rigid.AddForce(transform.up * jumpGauge, ForceMode2D.Impulse);
        ani.SetBool("isJumping", true);

        sound.clip = jump_landSound;
        sound.Play();
    }

    void PowerJump_Update()
    {
        LimitFallingSpeed();
        CheckGround();
        HorizontalMove();

        if (readyToRope())
        {
            ChangeState(States.AccessRope);
        }
        else if (readyToLand())
        {
            ChangeState(States.Land);
        }
    }

    void Jump_Exit()
    {
        ani.SetBool("isJumping", false);        
    }

    void PowerJump_Exit()
    {
        ani.SetBool("isJumping", false);        
    }

    void Fall_Enter()
    {
        transform.parent = null;
        ani.SetBool("isFalling", true);
    }

    void Fall_Update()
    {
        LimitFallingSpeed();
        CheckGround();
        HorizontalMove();

        if (readyToRope())
        {
            ChangeState(States.AccessRope);
        }
        else if (readyToLand())
        {
            ChangeState(States.Land);
        }
    }

    void Fall_Exit()
    {
        ani.SetBool("isFalling", false);
    }

    void Land_Enter()
    {
        jumpGauge = minJumpPower;
        ani.SetBool("isLanding", true);
        sound.clip = jump_landSound;
        sound.Play();
    }

    void Land_Update()
    {
        HorizontalMove();
        ChargeJumpGauge();

        if (readyToLever())
        {
            ChangeState(States.AccessLever);
        }
        else if (readyToRope())
        {
            ChangeState(States.AccessRope);
        }
        else if (readyToJump())
        {
            ChangeState(States.Jump);
        }
        else if (ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) // Land 애니메이션이 끝나면 Walk State로 전환
        {
            ChangeState(States.Walk);
        }
    }

    void Land_Exit()
    {
        ani.SetBool("isLanding", false);
    }

    void AccessRope_Enter()
    {        
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;
    }

    void AccessRope_Update()
    {
        // 이전 scene에서 rope에 매달린 상태로 현재 scene으로 넘어왔다면 rope를 새로 인식해야함
        if (rope == null) return;

        // 매달릴 위치 설정
        Vector2 destPos_rope;
        if (rope.CompareTag("VerticalRope"))
        {
            destPos_rope = new Vector2(rope.transform.position.x, transform.position.y);
        }
        else
        {
            destPos_rope = new Vector2(transform.position.x, rope.transform.position.y);
        }

        transform.position = Vector2.MoveTowards(transform.position, destPos_rope, Time.deltaTime * ropeAccessSpeed);
        if (Vector2.Distance(transform.position, destPos_rope) < 0.1f) // 플레이어가 rope에 근접했을 때
        {
            // 플레이어를 rope로 완전히 이동시킴
            transform.position = destPos_rope;
            transform.parent = rope.transform;
            ChangeState(States.MoveOnRope);
        }
    }

    void MoveOnRope_Enter()
    {
        jumpGauge = minJumpPower;

        // rope에 매달리는 애니메이션 실행
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
    }

    void MoveOnRope_Update()
    {
        ChargeJumpGauge();

        // Rope에 매달린 상태로 이동
        if (Physics2D.gravity.normalized == Vector2.left)
        {
            if (rope.CompareTag("VerticalRope"))
            {
                HorizontalRopeAni();
                rigid.velocity = new Vector2(0, -InputManager.instance.horizontal * ropeMoveSpeed);
            }
            else
            {
                VerticalRopeAni();
                rigid.velocity = new Vector2(InputManager.instance.vertical * ropeMoveSpeed, 0);
            }
        }
        else if (Physics2D.gravity.normalized == Vector2.right)
        {
            if (rope.CompareTag("VerticalRope"))
            {
                HorizontalRopeAni();
                rigid.velocity = new Vector2(0, InputManager.instance.horizontal * ropeMoveSpeed);
            }
            else
            {
                VerticalRopeAni();
                rigid.velocity = new Vector2(-InputManager.instance.vertical * ropeMoveSpeed, 0);
            }
        }
        else if (Physics2D.gravity.normalized == Vector2.up)
        {
            if (rope.CompareTag("VerticalRope"))
            {
                VerticalRopeAni();
                rigid.velocity = new Vector2(0, -InputManager.instance.vertical * ropeMoveSpeed);
            }
            else
            {
                HorizontalRopeAni();
                rigid.velocity = new Vector2(-InputManager.instance.horizontal * ropeMoveSpeed, 0);
            }
        }
        else
        {
            if (rope.CompareTag("VerticalRope"))
            {
                VerticalRopeAni();
                rigid.velocity = new Vector2(0, InputManager.instance.vertical * ropeMoveSpeed);
            }
            else
            {
                HorizontalRopeAni();
                rigid.velocity = new Vector2(InputManager.instance.horizontal * ropeMoveSpeed, 0);
            }
        }

        // Rope에서 점프
        if (readyToJump())
        {
            ChangeState(States.Jump);
        }
    }

    void MoveOnRope_Exit()
    { 
        transform.parent = null;
        rigid.gravityScale = defaultGravityScale;
        ani.SetBool("isRopingVerticalIdle", false);
        ani.SetBool("isRopingHorizontalIdle", false);
        ani.SetBool("isRopingVerticalMove", false);
        ani.SetBool("isRopingHorizontalMove", false);
    }

    void AccessLever_Enter()
    {
        rigid.velocity = Vector2.zero;
        
        // 레버 방향으로 플레이어 sprite flip
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
                break;
        }
    }

    void AccessLever_Update()
    {
        Vector2 destPos_beforeLevering;

        // 레버를 돌리기 위해 플레이어가 이동해야할 position 설정
        switch (lever.transform.eulerAngles.z)
        {
            case 0f: case 180f:
                destPos_beforeLevering = new Vector2(lever.transform.position.x, transform.position.y);
                break;

            default:
                destPos_beforeLevering = new Vector2(transform.position.x, lever.transform.position.y);
                break;
        }

        // 이동
        transform.position = Vector2.MoveTowards(transform.position, destPos_beforeLevering, 3f * Time.deltaTime);

        if ((Vector2)transform.position == destPos_beforeLevering)
        {
            ChangeState(States.SelectGravityDir);
        }
    }

    void SelectGravityDir_Enter()
    {
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }

    void SelectGravityDir_Update()
    {
        // 레버 돌림
        if (InputManager.instance.horizontalDown)
        {
            ChangeState(States.ChangeGravityDir);
        }
        // 레버 돌리기 취소
        else if (InputManager.instance.verticalDown)
        {
            ChangeState(States.Walk);
        }
    }

    void SelectGravityDir_Exit()
    {
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
    }

    void ChangeGravityDir_Enter()
    {
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;

        // 플레이어의 부모 오브젝트가 있다면 해제. 그렇지 않으면 플레이어와 함께 회전함
        transform.parent = null;
        
        // 회전해야할 플레이어 rotation 설정
        destRot = transform.eulerAngles.z + InputManager.instance.horizontal * 90f;
        if (destRot == -90f) destRot = 270f;

        // 회전하면서 바뀌어야할 플레이어 position 설정
        switch (lever.transform.eulerAngles.z)
        {
            case 0f: case 180f:
                destPos_afterLevering = new Vector2(transform.position.x, lever.transform.position.y);
                break;
            case 90f: case 270f:
                destPos_afterLevering = new Vector2(lever.transform.position.x, transform.position.y);
                break;
        }
        
        ani.SetBool("isFalling", true);
    }

    void ChangeGravityDir_Update()
    {
        // 레버 조작 후 플레이어 이동 및 회전
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, destPos_afterLevering, leverMoveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.forward * destRot), leverRotateSpeed * Time.deltaTime);
        
        // 플레이어 회전이 거의 완료되었다면 다음 state로 이동
        if (Mathf.RoundToInt(transform.eulerAngles.z) == destRot)
        {
            // 플레이어 완전히 회전
            if (destRot == 360f) destRot = 0f;
            transform.eulerAngles = Vector3.forward * destRot;

            // 중력 방향 변화
            Vector2 gravity = -transform.up * 9.8f;
            if (Mathf.Abs(gravity.x) < 1f) gravity.x = 0f;
            else gravity.y = 0f;
            Physics2D.gravity = gravity;

            // 플레이어에게 중력 적용
            rigid.gravityScale = defaultGravityScale;

            ChangeState(States.FallAfterLevering);
        }   
    }

    void FallAfterLevering_Update()
    {
        CheckGround();
        LimitFallingSpeed();   

        if (readyToLand())
        {
            ChangeState(States.Land);
        }
    }

    void FallAfterLevering_Exit()
    {
        ani.SetBool("isFalling", false);   
    }

    void GhostUsingLever_Enter()
    {
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;
    }

    void GhostUsingLever_Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, 0f, 0f), leverRotateSpeed * Time.deltaTime);
        
        // 플레이어 rotation이 거의 (0, 0, 0)이면 다음 state로 이동
        int angle = Mathf.RoundToInt(transform.eulerAngles.z);
        if (angle == 0 || angle == 360 )
        {
            ChangeState(States.FallAfterGhostLevering);    
        }
    }

    void FallAfterGhostLevering_Enter()
    {
        // 플레이어 완전히 회전
        transform.eulerAngles = Vector3.zero;

        Physics2D.gravity = new Vector2(0, -9.8f);
        rigid.gravityScale = defaultGravityScale;
    }

    void FallAfterGhostLevering_Update()
    {
        LimitFallingSpeed();
        CheckGround();

        if (isGrounded || isOnJumpPlatform)
        {
            ChangeState(States.Land);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        // 모든 스테이지에서 Spike와 부딫히면 사망
        if (other.gameObject.CompareTag("Spike")) Die();

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2: case 6: case 7:
                // 스테이지 2: Cannon, Arrow와 부딫히면 사망
                // 스테이지 6: 떨어지는 Fire와 부딫히면 사망
                // 스테이지 7: Bullet과 부딫히면 사망
                if (other.gameObject.CompareTag("Projectile")) Die();
                break;
            case 8:
                // 스테이지 8: Devil, Devil이 쏘는 레이저와 부딫히면 사망
                if (other.collider.CompareTag("Devil") || other.collider.CompareTag("Projectile")) Die();
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
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

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 3:
                if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) isHorizontalWind = true;
                else if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) isVerticalWind = true;
                break;
            case 4:
                // 스테이지 4 : 투명하지 않은 Ghost와 부딫히면 사망
                if (other.CompareTag("Ghost") && other.GetComponent<SpriteRenderer>().color.a != 0f) Die();
                break;
            case 6:
                // 스테이지 6 : 얼음 위에 있는 Fire와 부딫히면 사망
                if (other.CompareTag("Fire")) Die();
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 스테이지 3에서 Rope에 매달려 있지 않은 상태로 바람에 맞으면 날아감
        if (GameManager.instance.gameData.curStageNum == 3 && curState != States.AccessRope && curState != States.MoveOnRope)
        {
            if (other.CompareTag("UpWind")) rigid.AddForce(Vector2.up * windForce, ForceMode2D.Force);
            else if (other.CompareTag("DownWind")) rigid.AddForce(Vector2.down * windForce, ForceMode2D.Force);
            else if (other.CompareTag("RightWind")) rigid.AddForce(Vector2.right * windForce, ForceMode2D.Force);
            else if (other.CompareTag("LeftWind")) rigid.AddForce(Vector2.left * windForce, ForceMode2D.Force);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VerticalRope") || other.CompareTag("HorizontalRope")) isCollideRope = false;
        if (other.CompareTag("Lever")) isCollideLever = false;

        if (GameManager.instance.gameData.curStageNum == 3)
        {
            if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) isHorizontalWind = false;
            if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) isVerticalWind = false;
        }
    }

    void Die()
    {
        GameManager.instance.shouldStartAtSavePoint = true; //죽으면 일단 세이브포인트에 시작해야 함 
        UIManager.instance.FadeOut();
    }

    void HorizontalMove()
    {
        // Walk 애니메이션
        if (InputManager.instance.horizontal != 0)
        {           
            ani.SetBool("isWalking", true);
        }
        else ani.SetBool("isWalking", false);

        // Walk 방향에 따라 Player sprite 좌우 flip
        if (InputManager.instance.horizontal == 1) sprite.flipX = false;
        else if (InputManager.instance.horizontal == -1) sprite.flipX = true;

        // 이동 로직
        int stageNum = GameManager.instance.gameData.curStageNum;
        if (stageNum == 3 && (isHorizontalWind && Physics2D.gravity.x == 0f || isVerticalWind && Physics2D.gravity.y == 0f))
        {
            // Stage3에서 불어오는 바람과 같은 축으로는 플레이어가 키를 눌러도 이동 불가.
            return;
        }
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        if (stageNum == 6 && rayHitIcePlatform.collider != null && InputManager.instance.horizontal == 0f)
        {
            // Stage6에서 얼음 위라면 키보드 input이 끝난 후에 미끄러짐
            locVel = new Vector2(Vector2.Lerp(locVel, Vector2.zero, Time.deltaTime * slidingDegree).x , locVel.y);
        }
        else
        {
            // 얼음 위가 아니라면 키보드 input이 있을 때에만 이동
            locVel = new Vector2(InputManager.instance.horizontal * walkSpeed, locVel.y);
        }
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void LimitFallingSpeed()
    {
        // 떨어질 때 최대 속도 제한
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        if (locVel.y < -maxFallingSpeed) locVel.y = -maxFallingSpeed;
        rigid.velocity = transform.TransformDirection(locVel);
    }
 
    void CheckGround()
    {
        // 플레이어가 플랫폼과 함께 움직여야 하는 상황
        // 1) 움직이는 플랫폼 위에 있을 경우. 단, 움직이는 플랫폼 위에서 레버를 작동시킬 때는 함께 움직이면 안됨
        // 2) 움직이는 플랫폼과 연결된 rope에 매달려 있는 경우
        RaycastHit2D rayHitMovingFloor = Physics2D.BoxCast(transform.position, new Vector2(0.5f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 16);
        if (rayHitMovingFloor.collider != null && curState != States.ChangeGravityDir)
        {
            transform.parent = rayHitMovingFloor.collider.transform;
        }
        else if (curState != States.MoveOnRope)
        {
            transform.parent = null;
        }

        RaycastHit2D rayHit;
        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2:
                // Platform, Launcher, Stone 감지
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 6 | 1 << 15);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
            case 5:
                // Platform, JumpBoost 감지
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                rayHitJumpBoost = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 22);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                isOnJumpPlatform = rayHitJumpBoost.collider != null; //점프 강화 발판 위에 있는 것과 일반 플랫폼 위에 있는 것 구분                
                break;
            case 6:
                // Platform, IcePlatform 감지
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                rayHitIcePlatform = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 9);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null || rayHitIcePlatform.collider;
                break;
            case 7:
                // Platform, Enemy 감지
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 18);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
            default:
                // Platform 감지
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
        }
    }

    // 플레이어 기준 Vertical (월드 좌표 기준 X)
    void VerticalRopeAni()
    {
        sound.clip = moveSound;
        sound.Play();

        if (InputManager.instance.vertical == 0)
        {
            ani.SetBool("isRopingVerticalIdle", true);
            ani.SetBool("isRopingVerticalMove", false);
        }
        else
        {
            ani.SetBool("isRopingVerticalIdle", false);
            ani.SetBool("isRopingVerticalMove", true);
        }
    }

    // 플레이어 기준 Horizontal (월드 좌표 기준 X)
    void HorizontalRopeAni()
    {
        sound.clip = moveSound;
        sound.Play();

        if (InputManager.instance.horizontal == 0)
        {
            ani.SetBool("isRopingHorizontalIdle", true);
            ani.SetBool("isRopingHorizontalMove", false);
        }
        else
        {
            ani.SetBool("isRopingHorizontalIdle", false);
            ani.SetBool("isRopingHorizontalMove", true);
        }
        
        // 이동 방향에 따라 Player sprite 좌우 flip
        if (InputManager.instance.horizontal == 1) sprite.flipX = false;
        else if (InputManager.instance.horizontal == -1) sprite.flipX = true;
    }    

    /************* 아래는 Stage8 코드 ****************/
    /****** Stage8 기믹 수정할 때 코드 수정 필요 ******/
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
                if (mainCamera.shakedZ > 0f)
                {
                    mainCamera.shakedZ -= 0.5f;
                }
                else
                {
                    mainCamera.shakedZ += 0.5f;
                }
            }
            yield return null;
        }
        transform.eulerAngles = Vector3.forward * targetRot;
        mainCamera.shakedZ = 0f;

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
        rigid.gravityScale = defaultGravityScale;
        while (!isGrounded && !isBlackHole)
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
        collide.enabled = true;
        while (sprite.color.a < 1f)
        {
            Color color = sprite.color;
            color.a += 0.1f;
            sprite.color = color;
            yield return new WaitForSeconds(0.1f);
        }

        // Fall
        isBlackHoleFalling = true;
        isBlackHole = false;
        if (!isDevilRotating || isDevilFalling)
        {
            rigid.gravityScale = defaultGravityScale;
        }
        while (!isGrounded)
        {
            yield return null;
        }
        isBlackHoleFalling = false;
    }
}