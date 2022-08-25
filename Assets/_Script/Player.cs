using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MonsterLove.StateMachine;

using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{   
    //플레이어 죽을 때 파편 튀는 것 구현 
    public GameObject[] parts = new GameObject[4];
    bool isDieCorWork;

    [SerializeField] float defaultGravityScale;
    [SerializeField] float maxFallingSpeed;
    [SerializeField] public float walkSpeed;
    [SerializeField] public float maxWindSpeed; //stage3에서 바람에 날아갈 때 최대 속도 
    [SerializeField] float minJumpPower;
    [SerializeField] float maxJumpPower;
    [SerializeField] float maxJumpBoostPower;
    [SerializeField] float jumpChargeSpeed; // 점프 게이지 차는 속도
    [SerializeField] float ropeAccessSpeed; // rope에 접근하는 속도
    [SerializeField] float ropeMoveSpeed; // rope에 매달려 움직이는 속도
    [SerializeField] float leverRotateDelay; // lever 작동 후 플레이어가 회전하는 데 걸리는 시간 
    public float windForce; // Stage3의 바람에 의해 받는 힘~> 환풍기마다 다르게 설정할 수 있도록 외부접근 허용 
    [SerializeField] float slidingDegree; // Stage6의 얼음 위에서 미끄러지는 정도

    public bool isPlayerInSideStage; 
    //플레이어가 사이드 스테이지 내에 있을 때는 죽고 나면 별도의 respawnPos에서 새로 부활

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
    Func<bool> readyToFall, readyToLand, readyToJump, readyToPowerJump, readyToRope, readyToLever , readyToPowerLever; // 각 상태로 이동하기 위한 기본 조건

    // Walk
    public bool isPlayerExitFromWInd; //플레이어가 stage3 windZone에서 빠져나온 직후 아직 관성의 영향을 받고 있을 때 true 

    // Jump
    [SerializeField]float jumpGauge;
    [SerializeField] float jumpTimer; //땅에 닿기 직전 스페이스바를 눌러도 일정 오차범위 내에 있다면 점프명령 인식해야 함 
    public float jumpTimerOffset; //기준점 
    public float jumpHeightCut; //점프하다가 스페이스바에서 손을 떼면 점프방향 속도를 줄임

    [SerializeField] float groundedRemember; //플랫폼에서 떨어진 직후에도 오차범위 이내 시간에서는 점프할 수 있어야 함
    [SerializeField] float groundedRememberOffset;

    // Rope
    bool isCollideRope;
    GameObject rope; // 매달릴 rope

    // Lever
    bool isCollideLever;
    public bool shouldRotateHalf; //powerLever 가 아니면 90도만 회전, powerLever 이면 180도 회전 
    GameObject lever; // 작동시킬 lever
    Vector3 destPos_afterLevering; // Lever 작동 후 플레이어 position
    [SerializeField] int destRot; // Lever 작동 후 플레이어가 회전하고자 하는 각도
    float destGhostRot; //스테이지4의 유령이 레버를 돌릴 때 필요한 변수 

    // Wind
    bool isHorizontalWind;
    bool isVerticalWind;

    // 플레이어 아래에 있는 플랫폼 감지
    [SerializeField]public bool isGrounded;
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

    //오디오 소스 
    AudioSource sound;
    [SerializeField] AudioClip moveSound; //걷기 or 로프를 탈 때 나는 소리
    [SerializeField] AudioClip jump_landSound; //점프 및 착지할 때 나는 소리 

    GameObject cameraObj;
    public bool isCameraShake;

    [SerializeField] bool isLeftRayHit;
    [SerializeField] bool isRightRayHit;

    void Awake()
    {
        Time.timeScale = 1;
        isDieCorWork = false;

        fsm = StateMachine<States>.Initialize(this);
        rigid = GetComponent<Rigidbody2D>();
        collide = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<MainCamera>();
        openingSceneElevator = GameObject.Find("openingSceneElevator");
        sound = GetComponent<AudioSource>();
        cameraObj = GameObject.FindWithTag("MainCamera");

        for(int index=0; index<parts.Length; index++)
        {
            parts[index].SetActive(false);
        }
    }

    void Start()
    {
        sprite.color = new Color(1, 1, 1, 1); //플레이어 색상 초기화 

        // 각 State로 넘어가기 위한 기본 조건
        readyToFall = () => (!isGrounded)&&(!isOnJumpPlatform); //땅이나 점프강화발판 둘 다에 닿아있지 않을 때 
        readyToLand = () => (isGrounded || isOnJumpPlatform) && (int)transform.InverseTransformDirection(rigid.velocity).y <= 0; //땅or점프강화발판에 닿아 있고 y방향 속도벡터의 방향이 -1일 때 

        readyToJump = () => (jumpTimer > 0) && (!isOnJumpPlatform) && (groundedRemember > 0); //스페이스바를 눌렀고 강화점프 발판 위에 있지 않을 때 

        readyToPowerJump = () =>  InputManager.instance.jumpUp && (isOnJumpPlatform) && (groundedRemember > 0); //강화점프 발판 위에서 스페이스바를 눌렀다 뗄 때
        readyToRope = () => isCollideRope && InputManager.instance.vertical == 1; //위쪽 화살표 누르고 있고 + 로프에 닿아있을 때
        readyToLever = () => isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown 
                             && lever.transform.up == transform.up;
                             //레버에 닿아 있고, 레버와 동일한 rotation을 가지고 있고, 위쪽 화살표를 누르고 있을 때 

        // Scene이 세이브 포인트에서 시작하지 않을 때 플레이어 데이터 설정
        // 현재 Scene으로 넘어오기 직전의 데이터를 불러와서 적용
        if (!GameManager.instance.shouldStartAtSavePoint)
        {
            //GameManager ~> 씬이 시작할 때 플레이어의 위치 조정 
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
            /*
            if (GameManager.instance.gameData.curStageNum == 8 && GameManager.instance.gameData.curAchievementNum == 33 && !GameManager.instance.isCliffChecked)
            {
                InputManager.instance.isInputBlocked = true;
            }
            */
        }

        if (GameManager.instance.isStartWithFlipX)
        {
            sprite.flipX = true;
        }else
        {
            sprite.flipX = false;
        }

        jumpTimer = 0; //jumpTimer 초기화 

        //플레이어가 0에서 360 사이의 rotation 만을 가지도록 초기화해줌 
        /*
        if (transform.up == new Vector3(0, 1, 0)) transform.rotation = Quaternion.Euler(0, 0, 0);
        else if(transform.up == new Vector3(1, 0, 0)) transform.rotation = Quaternion.Euler(0, 0, 90f);
        else if (transform.up == new Vector3(0, -1, 0)) transform.rotation = Quaternion.Euler(0, 0, 180f);
        else transform.rotation = Quaternion.Euler(0, 0, 270f);
        */
       
        UIManager.instance.FadeIn(1.5f);
    }

    private void Update()
    {
        walkSoundCheck();
        AnimationManager();

        jumpTimer -= Time.deltaTime; //jumpTimer 매 프레임마다 작동 
        groundedRemember -= Time.deltaTime;
        if (InputManager.instance.jumpDown)
        {
            jumpTimer = jumpTimerOffset;
            //groundedRemember = 0;
        }
        if (isGrounded)
        {
            groundedRemember = groundedRememberOffset;
        }   
        
        if(rayPosHit_left.collider != null)
        {
            isLeftRayHit = true;
        }
        else
        {
            isLeftRayHit = false;
        }

        if(rayPosHit_right.collider != null)
        {
            isRightRayHit = true;
        }
        else
        {
            isRightRayHit = false;
        }
    }

    public void ChangeState(in States nextState)
    {
        fsm.ChangeState(nextState);
        curState = nextState;
    }

    void walkSoundCheck()
    {
        if(fsm.State == States.Walk && (isGrounded || isOnJumpPlatform))
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

    public float jumpPower;

    void Jump_Enter()
    {
        jumpTimer = 0;
        rigid.velocity = Vector2.zero; // 바닥 플랫폼의 속도가 점프 속도에 영향을 미치는 것을 방지    
        
        Vector2 addForceDirection;
        
        if (transform.up == new Vector3(0, 1, 0)) //플레이어가 위쪽을 향해 서 있을 때 
        {
            addForceDirection = new Vector2(0, 1);
        }
        else if (transform.up == new Vector3(0, -1, 0)) //플레이어가 아래쪽을 향해 서 있을 때 
        {
            addForceDirection = new Vector2(0, -1);
        }
        else if (transform.up == new Vector3(1, 0, 0)) //플레이어가 오른쪽을 향해 서 있을 때 
        {
            addForceDirection = new Vector2(1, 0);
        }
        else //플레이어가 왼쪽을 향해 서 있을 때
        {
            addForceDirection = new Vector2(-1, 0);
        }

        rigid.AddForce(jumpPower * addForceDirection, ForceMode2D.Impulse);

        sound.clip = jump_landSound;
        sound.Play();
    }
    /*
    IEnumerator jumpAddForce()
    {
        int index = 0;

        while (true)
        {
            Vector2 addForceDirection;
            float jumpPower = defaultJumpSpeed - index * speedDelta;

            if (transform.up == new Vector3(0, 1, 0)) //플레이어가 위쪽을 향해 서 있을 때 
            {
                addForceDirection = new Vector2(rigid.velocity.x, jumpPower);
            }
            else if (transform.up == new Vector3(0, -1, 0)) //플레이어가 아래쪽을 향해 서 있을 때 
            {
                addForceDirection = new Vector2(rigid.velocity.x, -jumpPower);
            }
            else if (transform.up == new Vector3(1, 0, 0)) //플레이어가 오른쪽을 향해 서 있을 때 
            {
                addForceDirection = new Vector2(jumpPower, rigid.velocity.y);
            }
            else //플레이어가 왼쪽을 향해 서 있을 때
            {
                addForceDirection = new Vector2(-jumpPower, rigid.velocity.y);
            }

            rigid.velocity = addForceDirection;
            Debug.Log(jumpPower + "," + index + "," + defaultJumpTimer);
            yield return new WaitForSeconds(0.005f);

            index++;
            defaultJumpTimer += 0.005f;
            if(defaultJumpTimer >= defaultJumpDelay || InputManager.instance.jumpUp)
            {
                break;
            }
        }        
    }
    */

    void Jump_Update()
    {
        LimitFallingSpeed();
        CheckGround();
        HorizontalMove();

        if (InputManager.instance.jumpUp) //스페이스바에서 손을 떼면 점프 방향 속도 줄여야 함 
        {
            if (transform.up.normalized == new Vector3(0, 1, 0) && rigid.velocity.y > 0) //플레이어가 위쪽을 바라보고 있을 때 + 속도가 높아지는 중일 때
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * jumpHeightCut, 0); //점프방향 속도 줄임 
            }
            else if (transform.up.normalized == new Vector3(0, -1, 0) && rigid.velocity.y < 0) //플레이어가 아래쪽을 바라보고 있을 때 
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * jumpHeightCut, 0);
            }
            else if (transform.up.normalized == new Vector3(1, 0, 0) && rigid.velocity.x > 0) //플레이어가 오른쪽을 바라보고 있을 때
            {
                rigid.velocity = new Vector3(rigid.velocity.x * jumpHeightCut, rigid.velocity.y, 0);
            }
            else if (transform.up.normalized == new Vector3(-1, 0, 0) && rigid.velocity.x < 0)//플레이어가 왼쪽을 바라보고 있을 때 
            {
                rigid.velocity = new Vector3(rigid.velocity.x * jumpHeightCut, rigid.velocity.y, 0);
            }

        }

        if (readyToRope())
        {
            ChangeState(States.AccessRope);
        }
        else if (readyToLand())
        {
            ChangeState(States.Land);
        }
    }

    void PowerJump_Enter()
    {
        rigid.velocity = Vector2.zero; // 바닥 플랫폼의 속도가 점프 속도에 영향을 미치는 것을 방지
        rigid.AddForce(transform.up * jumpGauge, ForceMode2D.Impulse);

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
    }

    void PowerJump_Exit()
    {     
    }

    void Fall_Enter()
    {
        groundedRemember = groundedRememberOffset;
        transform.parent = null;
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
            ChangeState(States.Land); //착지할 수 있는 조건이면 바로 Walk 상태로 전환 
        }else if (readyToJump())
        {
            ChangeState(States.Jump); //오차범위 내에서라면 플랫폼에서 떨어지기 시작한 직후에도 점프 가능
        }
    }

    void Fall_Exit()
    {

    }

    
    void Land_Enter()
    {
        jumpGauge = minJumpPower;      
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
        else // Land 애니메이션이 끝나면 Walk State로 전환
        {
            ChangeState(States.Walk);
        }
    }

    void Land_Exit()
    {

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

        
    }

    void MoveOnRope_Update()
    {
        //ChargeJumpGauge();
        groundedRemember = groundedRememberOffset;

        // Rope에 매달린 상태로 이동
        if (Physics2D.gravity.normalized == Vector2.left)
        {
            if (rope.CompareTag("VerticalRope"))
            {
                rigid.velocity = new Vector2(0, -InputManager.instance.horizontal * ropeMoveSpeed);
            }
            else
            {
                rigid.velocity = new Vector2(InputManager.instance.vertical * ropeMoveSpeed, 0);
            }
        }
        else if (Physics2D.gravity.normalized == Vector2.right)
        {
            if (rope.CompareTag("VerticalRope"))
            {
                rigid.velocity = new Vector2(0, InputManager.instance.horizontal * ropeMoveSpeed);
            }
            else
            {
                rigid.velocity = new Vector2(-InputManager.instance.vertical * ropeMoveSpeed, 0);
            }
        }
        else if (Physics2D.gravity.normalized == Vector2.up)
        {
            if (rope.CompareTag("VerticalRope"))
            {
                rigid.velocity = new Vector2(0, -InputManager.instance.vertical * ropeMoveSpeed);
            }
            else
            {
                rigid.velocity = new Vector2(-InputManager.instance.horizontal * ropeMoveSpeed, 0);
            }
        }
        else
        {
            if (rope.CompareTag("VerticalRope"))
            {
                rigid.velocity = new Vector2(0, InputManager.instance.vertical * ropeMoveSpeed);
            }
            else
            {
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
        float moveToLeverSpeed = 9f;
        transform.position = Vector2.MoveTowards(transform.position, destPos_beforeLevering, moveToLeverSpeed * Time.deltaTime);        

        if ((Vector2)transform.position == destPos_beforeLevering)
        {
            ChangeState(States.SelectGravityDir);
        }
    }
   
    void SelectGravityDir_Enter()
    {
        if (!shouldRotateHalf) //180도 회전하는 강화레버의 경우 화살표를 띄우지 않음 
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }       
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
        if (!shouldRotateHalf)
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
        }     
    }

    void ChangeGravityDir_Enter()
    {
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;

        // 플레이어의 부모 오브젝트가 있다면 해제. 그렇지 않으면 플레이어와 함께 회전함
        transform.parent = null;

        // 회전해야할 플레이어 rotation 설정
        if (!shouldRotateHalf) //90도 회전하는 일반적인 경우 
        {           
            if(InputManager.instance.horizontal == 1) //z rotation + 90 degree (반시계방향 회전)
            {
                destRot = 90;
            }
            else if (InputManager.instance.horizontal == -1)
            {
                destRot = -90;
            }
        }
        else //180도 회전하는 경우 
        {
            if (!sprite.flipX)
            {
                destRot = 180;
            }
            else
            {
                destRot = -180;
            }
            //180도 회전 레버의 경우 플레이어가 바라보는 방향에 따라 회전방향이 달라진다 
        }

        // 회전하면서 바뀌어야할 플레이어 position 설정
        if (!shouldRotateHalf) //90도회전시
        {
            switch (lever.transform.eulerAngles.z)
            {
                case 0f:  case 180f:
                    destPos_afterLevering = new Vector2(transform.position.x, lever.transform.position.y);
                    break;
                case 90f: case 270f:               
                    destPos_afterLevering = new Vector2(lever.transform.position.x, transform.position.y);
                    break;
            }
        }
        else //180도 회전시 
        {
            switch (lever.transform.eulerAngles.z)
            {
                case 0f:  case 180f:               
                    destPos_afterLevering = new Vector2(transform.position.x, transform.position.y);
                    break;
                case 90f: case 270f:               
                    destPos_afterLevering = new Vector2(transform.position.x, transform.position.y);
                    break;
            }
        }

        timer = 0;
        initZRot = Mathf.RoundToInt(transform.eulerAngles.z); //물리엔진 연산오차로 인한 버그를 막기 위해 int 형으로 반올림 

        Time.timeScale = 0; //플레이어가 회전하는동안 시간 멈춤 

        cameraObj.GetComponent<MainCamera>().cameraShake(0.3f, 0.5f);
    }

    float timer=0;
    float initZRot;
    void ChangeGravityDir_Update()
    {
        if (isCameraShake) return; //카메라 흔들림이 끝나고 나서 플레이어 회전 

        // 레버 조작 후 플레이어 이동 및 회전
        cameraObj.transform.position = cameraObj.GetComponent<MainCamera>().cameraPosCal();
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, destPos_afterLevering, Time.unscaledDeltaTime / leverRotateDelay);
       
        float newRotZ = transform.eulerAngles.z + destRot * Time.unscaledDeltaTime / leverRotateDelay;
        transform.rotation = Quaternion.Euler(0, 0, newRotZ);
        timer += Time.unscaledDeltaTime;

        if(timer >= leverRotateDelay)
        {
            rotationCorrect();
        }
    }

    void rotationCorrect()
    {
        transform.rotation = Quaternion.Euler(0, 0, initZRot + destRot);

        // 중력 방향 변화
        Vector2 gravity = -transform.up * 9.8f;
        if (Mathf.Abs(gravity.x) < 1f) gravity.x = 0f; //물리엔진 연산오차 보정(0.00xxx 같이 나올 경우 0으로 고정해줘야 함) 
        else gravity.y = 0f;

        Physics2D.gravity = gravity; //맵 전체 중력방향 바꿈 

        // 플레이어에게 중력 적용
        rigid.gravityScale = defaultGravityScale;

        ChangeState(States.FallAfterLevering);
    }

    void FallAfterLevering_Enter()
    {
        Time.timeScale = 1;
        if (shouldRotateHalf) shouldRotateHalf = false;
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

    }

    void GhostUsingLever_Enter()
    {
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;

        if(transform.up == new Vector3(0, 1, 0))
        {
            destGhostRot = 0;
        }
        else  if(transform.up == new Vector3(-1, 0, 0))
        {
            destGhostRot = -90f;
        }
        else if(transform.up == new Vector3(1, 0, 0))
        {
            destGhostRot = 90f;
        }
        else
        {
            destGhostRot = 180f;
        }
    }

    void GhostUsingLever_Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, transform.rotation.z + (1 / leverRotateDelay * destGhostRot * Time.unscaledDeltaTime));

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
        if (other.gameObject.CompareTag("Spike") && !isDieCorWork)
        {
            StartCoroutine(Die());
        }

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2: case 6: case 7:
                // 스테이지 2: Cannon, Arrow와 부딫히면 사망
                // 스테이지 6: 떨어지는 Fire와 부딫히면 사망
                // 스테이지 7: Bullet과 부딫히면 사망
                if (other.gameObject.CompareTag("Projectile") && !isDieCorWork)
                {
                    StartCoroutine(Die());
                }
                break;
            case 8:
                // 스테이지 8: Devil, Devil이 쏘는 레이저와 부딫히면 사망
                if (other.collider.CompareTag("Devil") || other.collider.CompareTag("Projectile")) StartCoroutine(Die());
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
            if (lever.GetComponent<lever>().isPowerLever)
            {
                shouldRotateHalf = true;
            }
        }
        
        switch (GameManager.instance.gameData.curStageNum)
        {
            case 3:
                if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) isHorizontalWind = true;
                else if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) isVerticalWind = true;
                break;
            case 4:
                // 스테이지 4 : 투명하지 않은 Ghost와 부딫히면 사망
                if (other.CompareTag("Ghost") && other.GetComponent<SpriteRenderer>().color.a != 0f) StartCoroutine(Die());
                break;
            case 6:
                // 스테이지 6 : 얼음 위에 있는 Fire와 부딫히면 사망
                if (other.CompareTag("Fire")) StartCoroutine(Die());
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // 스테이지 3에서 Rope에 매달려 있지 않은 상태로 바람에 맞으면 날아감
        if (GameManager.instance.gameData.curStageNum == 3 && curState != States.AccessRope && curState != States.MoveOnRope)
        {
            if (other.CompareTag("UpWind"))
            {
                if (rigid.velocity.y >= maxWindSpeed)
                {
                    rigid.velocity = new Vector2(rigid.velocity.x, maxWindSpeed);
                    return;
                }
                rigid.AddForce(Vector2.up * windForce, ForceMode2D.Force);
            }
            else if (other.CompareTag("DownWind"))
            {
                if (rigid.velocity.y <= -maxWindSpeed)
                {
                    rigid.velocity = new Vector2(rigid.velocity.x, -maxWindSpeed);
                    return;
                }
                rigid.AddForce(Vector2.down * windForce, ForceMode2D.Force);
            }
            else if (other.CompareTag("RightWind"))
            {
                if (rigid.velocity.x >= maxWindSpeed)
                {
                    rigid.velocity = new Vector2(maxWindSpeed, rigid.velocity.y);
                    return;
                }
                rigid.AddForce(Vector2.right * windForce, ForceMode2D.Force);
            }
            else if (other.CompareTag("LeftWind"))
            {
                if (rigid.velocity.x <= -maxWindSpeed)
                {
                    rigid.velocity = new Vector2(-maxWindSpeed, rigid.velocity.y);
                    return;
                }
                rigid.AddForce(Vector2.left * windForce, ForceMode2D.Force);
            }            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("VerticalRope") || other.CompareTag("HorizontalRope")) isCollideRope = false;
        if (other.CompareTag("Lever"))
        {
            isCollideLever = false;
            if (shouldRotateHalf) shouldRotateHalf = false;
        }
        
        if (GameManager.instance.gameData.curStageNum == 3)
        {
            if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) isHorizontalWind = false;
            if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) isVerticalWind = false;
        }
    }

    IEnumerator Die()
    {
        isDieCorWork = true; //죽음 모션이 진행중일 땐 트리거가 발동하더라도 다시 죽지 않음 

        GameManager.instance.shouldStartAtSavePoint = true; //죽으면 일단 세이브포인트에서 시작해야 함 
        cameraObj.GetComponent<MainCamera>().isCameraLock = true; //카메라 움직이지 않게 고정
        cameraObj.GetComponent<MainCamera>().cameraShake(0.5f, 0.7f);
        sprite.color = new Color(1, 1, 1, 0); //잠시 플레이어 투명화 

        for(int index=0; index<parts.Length; index++)
        {
            parts[index].SetActive(true);
            Rigidbody2D rigid = parts[index].GetComponent<Rigidbody2D>();

            Vector2 randomDir = new Vector2(Random.insideUnitSphere.x, Random.insideUnitSphere.y);
            float randomPower = Random.Range(20f, 30f);

            rigid.AddForce(randomDir * randomPower, ForceMode2D.Impulse); //세 파츠에 랜덤 방향,크기의 힘을 가해서 튕겨냄 
        }
        yield return new WaitForSeconds(1.5f);

        UIManager.instance.FadeOut(1f); //화면 어두워지고
        yield return new WaitForSeconds(2f);

        isDieCorWork = false;
        GameManager.instance.StartGame(false); //새로 재시작 ~> 초기화 시 initData(false) 가 실행 
    }

    //애니메이션 변환 
    string curAnim;
    void changeAnimation(string newAnim)
    {
        if (newAnim == curAnim) return;
        if (newAnim != curAnim)
        {
            ani.Play(newAnim);
            curAnim = newAnim;
        }
    }

    float jumpAniThreshold = 1f;
    void AnimationManager() //플레이어 상태 전체의 애니메이션 관리 
    {
        if(fsm.State == States.Walk) // Walk, idle 애니메이션 ~> 플레이어가 지면에 닿아있을 때 
        {
            // Walk 방향에 따라 Player sprite 좌우 flip
            if (InputManager.instance.horizontal == 1) sprite.flipX = false;
            else if (InputManager.instance.horizontal == -1) sprite.flipX = true;

            if (InputManager.instance.horizontal == 0)
            {
                //플레이어의 두 발이 모두 땅에 닿아있을 때 
                if(rayPosHit_left.collider != null && rayPosHit_right.collider != null)
                {
                    changeAnimation("new_idle");
                }
                
                //플레이어의 두 발 중 하나만 땅에 닿아있을 때 
                if((rayPosHit_left.collider == null && rayPosHit_right.collider != null) || (rayPosHit_left.collider != null && rayPosHit_right.collider == null))
                {
                    changeAnimation("new_cliff");
                }              
            }
            else
            {
                changeAnimation("new_walk");               
            }
        }
        else if(fsm.State == States.Jump || fsm.State == States.Fall) // jump, fall ~> 플레이어가 공중에 떠 있을 때 
        {
            // Walk 방향에 따라 Player sprite 좌우 flip
            if (InputManager.instance.horizontal == 1) sprite.flipX = false;
            else if (InputManager.instance.horizontal == -1) sprite.flipX = true;

            float jumpVel = Vector3.Dot(rigid.velocity, transform.up); //+ 이면 플레이어가 local Y 방향으로 + 속도, - 이면 플레이어가 local Y 방향으로 - 속도 
            if(jumpVel >= jumpAniThreshold)
            {
                changeAnimation("new_floatUp");
            }
            else if (Mathf.Abs(jumpVel) < jumpAniThreshold)
            {
                changeAnimation("new_floatMiddle");
            }
            else
            {
                changeAnimation("new_floatDown");
            }
        }
        else if (fsm.State == States.AccessRope || fsm.State == States.MoveOnRope)
        {
            if (rope.CompareTag("VerticalRope")) //수직 로프일 때 
            {
                if(InputManager.instance.vertical == 0)
                {
                    changeAnimation("new_ropeVertical"); //로프 위에 정지해있음 
                }
                else
                {
                    changeAnimation("new_ropeVertical_move"); //로프 위에서 움직임 
                }
            }
            else if (rope.CompareTag("HorizontalRope")) //수평 로프일 때 
            {
                if(InputManager.instance.horizontal == 0)
                {
                    changeAnimation("new_ropeHorizontal"); //로프 위에 정지해있음 
                }
                else
                {
                    changeAnimation("new_ropeHorizontal_move"); //로프 위에서 움직임 
                }
            }
        }
        else if(fsm.State == States.AccessLever)
        {
            changeAnimation("new_idle");
        }
    }

    void HorizontalMove()
    {        
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
        else if (stageNum == 3 && isPlayerExitFromWInd)
        {
            //stage03 에서 환풍기 바람 영향을 받고 있을 때의 움직임은 windPower 스크립트에서 수행 
            return;
        }

        else //일반적인 케이스 
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

    //플레이어가 플랫폼 끝에 한 발로 서 있을때의 모션 설정을 위한 hit 
    RaycastHit2D rayPosHit_left; //왼쪽 
    RaycastHit2D rayPosHit_right; //오른쪽 

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
        
        Vector2 rayStartPos_left, rayStartPos_right;
        Vector2 middlePos = transform.position - transform.up; //(x,y) 두 좌표면 충분함 
        float centerToLeg = 0.3125f;

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
                //rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.875f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);

                isGrounded = rayPosHit_left.collider != null || rayPosHit_right.collider != null;
              
                if (transform.up == new Vector3(0, 1, 0)) // 머리가 위쪽을 향함 
                {
                    rayStartPos_left = middlePos + new Vector2(-centerToLeg, 0);
                    rayStartPos_right = middlePos + new Vector2(centerToLeg, 0);
                }
                else if(transform.up == new Vector3(1, 0, 0)) // 머리가 오른쪽을 향함
                {
                    rayStartPos_left = middlePos + new Vector2(0, centerToLeg);
                    rayStartPos_right = middlePos + new Vector2(0, -centerToLeg);
                }
                else if(transform.up == new Vector3(0, -1, 0)) //머리가 아래쪽을 향함 
                {
                    rayStartPos_left = middlePos + new Vector2(centerToLeg, 0);
                    rayStartPos_right = middlePos + new Vector2(-centerToLeg, 0);
                }
                else //머리가 왼쪽을 향함 
                {
                    rayStartPos_left = middlePos + new Vector2(0, -centerToLeg);
                    rayStartPos_right = middlePos + new Vector2(0, centerToLeg);
                }

                rayPosHit_left = Physics2D.Raycast(rayStartPos_left, -transform.up, 0.05f, LayerMask.GetMask("Platform"));
                rayPosHit_right = Physics2D.Raycast(rayStartPos_right, -transform.up, 0.05f, LayerMask.GetMask("Platform"));

                Debug.DrawRay(rayStartPos_left, -transform.up * 0.2f, new Color(1, 0, 0));
                Debug.DrawRay(rayStartPos_right, -transform.up * 0.2f, new Color(1, 0, 0));

                break;
        }
    }

    // 플레이어 기준 Vertical (월드 좌표 기준 X)

    /*
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
    */

    /************* 아래는 Stage8 코드 ****************/
    /****** Stage8 기믹 수정할 때 코드 수정 필요 ******/

    /*
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
    */
}