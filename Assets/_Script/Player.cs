using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MonsterLove.StateMachine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.IO;

public class Player : MonoBehaviour
{   
    //플레이어 사망 시 튕겨져 나가는 파편 오브젝트 
    public GameObject[] parts = new GameObject[4];
    public GameObject boxGrabColl;
    bool isDieCorWork;

    [SerializeField] float defaultGravityScale;
    [SerializeField] float maxFallingSpeed;
    [SerializeField] public float walkSpeed;
    [SerializeField] public float maxWindSpeed; //stage3에서 windHome 의 최대속도 
    [SerializeField] float minJumpPower;
    [SerializeField] float maxJumpPower;
    [SerializeField] float maxJumpBoostPower;
    [SerializeField] float jumpChargeSpeed; // 점프 게이지 채우는 속도 
    [SerializeField] float ropeAccessSpeed; // jump or walk 상태에서 로프에 매달릴 때 지정위치까지 이동하는 속도 
    [SerializeField] float ropeMoveSpeed; // rope 위에서 움직이는 속도 
    [SerializeField] float leverRotateDelay; // lever를 작동시킨 후 화면이 회전하는 데 걸리는 시간 
    public float windForce; // Stage3에서 windHome 이 플레이어를 가속시키는 가속력 
    [SerializeField] float slidingDegree; 

    public enum States
    {
        Walk, Fall, Land, Jump, Grab,
        PowerJump, // Stage5의 강화점프 
        AccessRope, MoveOnRope,
        AccessLever, SelectGravityDir, ChangeGravityDir, FallAfterLevering,
        GhostUsingLever, FallAfterGhostLevering // Stage4 기믹
    }
    StateMachine<States> fsm;
    public static States curState {get; private set;}
    Func<bool> readyToFall, readyToLand, readyToJump, readyToGrab, readyToPowerJump, readyToRope, readyToLever , readyToPowerLever;

    // Walk
    public bool isPlayerExitFromWInd; 
    //stage 3 에서 플레이어가 바람을 빠져나온 직후 땅에 닿기 전 까지는 가속력을 받으며 관성에 따른 운동 계속해야 함 

    // Jump
    [SerializeField]float jumpGauge;
    [SerializeField] float jumpTimer; 
    public float jumpTimerOffset; 
    public float jumpHeightCut; //플레이어가 체공중일 때 스페이스바를 떼면 플레이어의 속도가 줄어들면서 땅으로 떨어짐 

    [SerializeField] float groundedRemember; 
    [SerializeField] float groundedRememberOffset;

    // Rope
    bool isCollideRope;
    GameObject rope; //현재 플레이어가 매달려 있는 로프 ~> 동시 두개는 error 발생 

    // Lever
    bool isCollideLever;
    public bool shouldRotateHalf; //powerLever 이면 180도 회전 ~> true, powerLever 아니면 90도 회전 ~> false 
    GameObject lever; //현재 플레이어가 작동시키려는 lever
    Vector3 destPos_afterLevering; // Lever 작동직후 플레이어가 이동해야 할 position
    [SerializeField] int destRot; // Lever 작동후
    float destGhostRot; 

    // Grab
    public bool isPlayerGrab = false; //플레이어가 오브젝트를 잡고 있는지의 여부 

    // Wind
    bool isHorizontalWind;
    bool isVerticalWind;

    // 플레이어가 땅에 닿아있는지의 여부 
    [SerializeField]public bool isGrounded;
    [SerializeField] bool isOnJumpPlatform = false; 
    RaycastHit2D rayHitIcePlatform;
    RaycastHit2D rayHitJumpBoost;

    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject rightArrow;
    MainCamera mainCamera;
    Rigidbody2D rigid;
    BoxCollider2D collide;
    SpriteRenderer sprite;
    Animator ani;

    // Stage8 
    [HideInInspector] public bool isDevilRotating;
    [HideInInspector] public bool isBlackHole;
    bool isDevilFalling;

    //사운드 기믹 
    AudioSource sound;
    [SerializeField] AudioClip moveSound; 
    [SerializeField] AudioClip jump_landSound; 

    GameObject cameraObj;
    public bool isCameraShake;

    public bool isLand;

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
        sound = GetComponent<AudioSource>();
        cameraObj = GameObject.FindWithTag("MainCamera");

        for(int index=0; index<parts.Length; index++)
        {
            parts[index].SetActive(false);
        }
    }

    void Start()
    {
        InputManager.instance.isInputBlocked = false;
        InputManager.instance.isJumpBlocked = false;

        //플레이어가 각 state 로 전이하기 위한 조건 
        readyToFall = () => (!isGrounded)&&(!isOnJumpPlatform); //땅이나 강화발판에 닿아있지 않으면 
        readyToLand = () => (isGrounded || isOnJumpPlatform) && (int)transform.InverseTransformDirection(rigid.velocity).y <= 0; 
        //
        readyToGrab = () => isPlayerGrab; //isPlayerGrab 이 true 이면 오브젝트 잡기로 이동 

        readyToJump = () => (jumpTimer > 0) && (!isOnJumpPlatform) && (groundedRemember > 0);

        readyToPowerJump = () =>  InputManager.instance.jumpUp && (isOnJumpPlatform) && (groundedRemember > 0);
        readyToRope = () => isCollideRope && InputManager.instance.vertical == 1; //위쪽 화살표를 누르면서 로프 콜라이더에 닿아 있으면 
        readyToLever = () => isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown 
                             && lever.transform.up == transform.up;
        
        if (!GameManager.instance.shouldSpawnSavePoint)
        {
            //GameManager ~> 세이브포인트에서 시작하는 것이 아닐 때 GM이 플레이어 초기화 담당 ~> 스테이지 처음 시작할 때, 한 씬에서 통로를 통해 다음씬 넘어갈 때 
            transform.position = GameManager.instance.nextPos;
            Physics2D.gravity = GameManager.instance.nextGravityDir * 9.8f;
            transform.up = -GameManager.instance.nextGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z; // x-rotation, y-rotation은 자동으로 0 지정 

            // 로프에 매달린 상태로 넘어왔을 때, 떨어지면서 넘어왔을 때 ~> 해당 상태 유지해 줘야 함
            States nextState = GameManager.instance.nextState;
            if (nextState == States.MoveOnRope) ChangeState(States.AccessRope);
            else if (nextState == States.Jump) ChangeState(States.Fall);
            else ChangeState(nextState);
        }

        //세이브포인트에서 시작해야 할 때 ~> GameData 에서 플레이어 초기화 담당 
        else
        {
            transform.position = GameManager.instance.gameData.respawnPos;
            Physics2D.gravity = GameManager.instance.gameData.respawnGravityDir * 9.8f;
            transform.up = -GameManager.instance.gameData.respawnGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z;
            ChangeState(States.Walk); //세이브포인트에서 시작할 땐 항상 walk 상태로 시작 
            GameManager.instance.shouldSpawnSavePoint = false;
        }

        if (GameManager.instance.isStartWithFlipX)
        {
            sprite.flipX = true;
        }else
        {
            sprite.flipX = false;
        }

        jumpTimer = 0; //jumpTimer 초기화
        boxGrabColl.SetActive(false);
       
        UIManager.instance.FadeIn(1.5f);
    }

    private void Update()
    {
        isLand = readyToLand();

        walkSoundCheck();
        AnimationManager();

        jumpTimer -= Time.deltaTime; //jumpTimer 초기화
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
        else if (readyToJump())
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
        }else if (readyToGrab())
        {
            ChangeState(States.Grab);
        }
    }

    void ChargeJumpGauge() //stage5에서 사용하는 기믹 
    {
        if (InputManager.instance.jump)
        {
            if (rayHitJumpBoost.collider != null)
            {
                jumpGauge = Mathf.Clamp(jumpGauge + jumpChargeSpeed * Time.deltaTime, minJumpPower, maxJumpBoostPower);
            }            
        }
    }

    public float jumpPower;

    void Jump_Enter()
    {
        jumpTimer = 0;
        rigid.velocity = Vector2.zero; //움직이는 발판 위에서 점프하거나 할 때 속도 바뀌지 않도록 속도 초기화  
        
        Vector2 addForceDirection;
        
        if (transform.up == new Vector3(0, 1, 0)) //머리가 위를 바라봄
        {
            addForceDirection = new Vector2(0, 1);
        }
        else if (transform.up == new Vector3(0, -1, 0)) //머리가 아래를 바라봄
        {
            addForceDirection = new Vector2(0, -1);
        }
        else if (transform.up == new Vector3(1, 0, 0)) //머리가 오른쪽을 바라봄
        {
            addForceDirection = new Vector2(1, 0);
        }
        else //머리가 왼쪽을 바라봄
        {
            addForceDirection = new Vector2(-1, 0);
        }

        rigid.AddForce(jumpPower * addForceDirection, ForceMode2D.Impulse);

        sound.clip = jump_landSound;
        sound.Play();
    }
    
    public float fallingGravity;
    void Jump_Update()
    {
        LimitFallingSpeed();
        CheckGround();
        HorizontalMove();

        if (InputManager.instance.jumpUp) //플레이어가 점프 중 스페이스바에서 손을 떼면 속도 줄여서 점프 멈춰줌 
        {
            if (transform.up.normalized == new Vector3(0, 1, 0) && rigid.velocity.y > 0)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * jumpHeightCut, 0); 
            }
            else if (transform.up.normalized == new Vector3(0, -1, 0) && rigid.velocity.y < 0)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * jumpHeightCut, 0);
            }
            else if (transform.up.normalized == new Vector3(1, 0, 0) && rigid.velocity.x > 0) 
            {
                rigid.velocity = new Vector3(rigid.velocity.x * jumpHeightCut, rigid.velocity.y, 0);
            }
            else if (transform.up.normalized == new Vector3(-1, 0, 0) && rigid.velocity.x < 0)
            {
                rigid.velocity = new Vector3(rigid.velocity.x * jumpHeightCut, rigid.velocity.y, 0);
            }

            rigid.gravityScale = fallingGravity; //착지할 때 착! 달라붙는 느낌을 주기 위해 중력 강화함 
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
        rigid.velocity = Vector2.zero;
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
            ChangeState(States.Land); 
        }else if (readyToJump())
        {
            ChangeState(States.Jump); 
        }
    }

    void Fall_Exit()
    {

    }

    
    void Land_Enter()
    {
        rigid.gravityScale = 3f;        
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
        else // Land동작이 완료되면 자동으로 walk 로 넘어감 
        {
            ChangeState(States.Walk);
        }
    }

    void Land_Exit()
    {
        
    }
    
    void Grab_Update()
    {
        CheckGround();
        HorizontalMove();

        if (!isPlayerGrab) //만약 grab = false 가 되면 바로 walk 로 상태 넘어감 
        {
            ChangeState(States.Walk);
        }
        else if (readyToFall()) //물체를 들고있는 상태에서 점프는 불가능 but 떨어지는 것은 가능 
        {
            ChangeState(States.Fall);
        }
    }


    void AccessRope_Enter()
    {        
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;
    }

    void AccessRope_Update()
    {
        if (rope == null) return;

        // 로프에 매달리기 위해 이동해야 하는 위치 
        Vector2 destPos_rope;
        if (rope.CompareTag("VerticalRope")) //세로 로프일 때
        {
            destPos_rope = new Vector2(rope.transform.position.x, transform.position.y);
        }
        else //가로 로프일 때 
        {
            destPos_rope = new Vector2(transform.position.x, rope.transform.position.y-0.1f);
        }

        transform.position = Vector2.MoveTowards(transform.position, destPos_rope, Time.deltaTime * ropeAccessSpeed);
        if (Vector2.Distance(transform.position, destPos_rope) < 0.1f) // 플레이어가 목표위치에 근접하면 도달한 것으로 함 ~> 물리엔진 연산오차 보정 
        {
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
        groundedRemember = groundedRememberOffset;

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

        // Rope에서 점프할 때 
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
        if(leverColl != null)
        {
            leverColl.GetComponent<lever>().lightTurnOff();
            //플레이어가 레버를 작동시키려고 하면 레버 불을 끔 
        }

        //플레이어가 레버의 오른쪽에 있는지, 왼쪽에 있는지에 따라 sprite flip 바꿔줘야 함 
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

        //플레이어의 크기가 1x1 이 아니므로 레버를 돌릴 때 각도만 바뀌는 것이 아니라 pivot의 위치도 바뀌어야 함
        switch (lever.transform.eulerAngles.z)
        {
            case 0f: case 180f:
                destPos_beforeLevering = new Vector2(lever.transform.position.x, transform.position.y);
                break;

            default:
                destPos_beforeLevering = new Vector2(transform.position.x, lever.transform.position.y);
                break;
        }

        // 레버 돌리는 속도 
        float moveToLeverSpeed = 9f;
        transform.position = Vector2.MoveTowards(transform.position, destPos_beforeLevering, moveToLeverSpeed * Time.deltaTime);        

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
        // 좌우 화살표 누르면 ~> 실제 레버 작동단계로 이행
        if (InputManager.instance.horizontalDown)
        {
            ChangeState(States.ChangeGravityDir);
        }
        // 위 화살표 누르면 ~> 레버 작동 캔슬 
        else if (Input.GetKeyDown(KeyCode.UpArrow)) 
        {
            leverColl.GetComponent<lever>().lightTurnOn(); //다시 레버 불 킴 

            leftArrow.SetActive(false);
            rightArrow.SetActive(false);
            ChangeState(States.Walk);
        }
    }

    void SelectGravityDir_Exit()
    {
          
    }

    void ChangeGravityDir_Enter()
    {
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;

        transform.parent = null;

        if (!shouldRotateHalf) //90도 회전해야 하는 경우
        {           
            if(InputManager.instance.horizontal == 1) 
            {
                leftArrow.SetActive(false);
                destRot = -90;
            }
            else if (InputManager.instance.horizontal == -1)
            {
                rightArrow.SetActive(false); //각각 해당하는 방향의 화살표만 남기고 지우기 
                destRot = 90;
            }
        }
        else //180도 회전해야 하는 경우 
        {
            leftArrow.SetActive(false);
            rightArrow.SetActive(false);

            if (InputManager.instance.horizontal == 1)
            {
                destRot = -180;
            }
            else
            {
                destRot = 180;
            }
        }

        // 레버 회전이후 도달해야 하는 위치 조정 
        if (!shouldRotateHalf) //90도 회전하는 경우 
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
        else //180도 회전하는 경우 
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
        initZRot = Mathf.RoundToInt(transform.eulerAngles.z); //물리엔진 연산오차 보정을 위해 z 회전값을 정수로 반올림 

        Time.timeScale = 0; //레버 돌아가는 동안은 시간 정지  

        cameraObj.GetComponent<MainCamera>().cameraShake(0.3f, 0.5f);
    }

    float timer=0;
    float initZRot;
    void ChangeGravityDir_Update()
    {
        if (isCameraShake) return; //카메라 회전이 끝나고 레버회전 시작 

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

        Vector2 gravity = -transform.up * 9.8f;
        if (Mathf.Abs(gravity.x) < 1f) gravity.x = 0f; //물리엔진 연산오차 보정 ~> x 방향 중력값이 1미만 소수면 ~> 0으로 만들어 줌 
        else gravity.y = 0f; //만약 x 방향 중력값이 1이면 y 값으로는 중력이 없어야 함 ~> 0으로 만들어 줌 

        Physics2D.gravity = gravity; //맵 전체의 중력값을 플레이어 중력방향에 맞게 보정 

        // 중력 방향 지정 끝나면 크기를 설정값에 맞게 지정해 줌 
        rigid.gravityScale = defaultGravityScale;

        ChangeState(States.FallAfterLevering);
    }

    void FallAfterLevering_Enter()
    {
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);

        Time.timeScale = 1;
        if (shouldRotateHalf) shouldRotateHalf = false;
        InputManager.instance.isInputBlocked = true;
        leverColl.GetComponent<lever>().lightTurnOn(); //다시 레버 불 킴 
    }

    void FallAfterLevering_Update()
    {
        CheckGround();
        LimitFallingSpeed();

        if (readyToLand())
        {           
            InputManager.instance.isInputBlocked = false;
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

        int angle = Mathf.RoundToInt(transform.eulerAngles.z);
        if (angle == 0 || angle == 360 )
        {
            ChangeState(States.FallAfterGhostLevering);    
        }
    }

    void FallAfterGhostLevering_Enter()
    {
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
        // 어떤 스테이지든 spike에 닿으면 죽음 
        if (other.gameObject.CompareTag("Spike") && !isDieCorWork)
        {
            StartCoroutine(Die());
        }

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2: case 6: case 7:               
                if (other.gameObject.CompareTag("Projectile") && !isDieCorWork)
                {
                    StartCoroutine(Die());
                }
                break;
            case 8:
                if (other.collider.CompareTag("Devil") || other.collider.CompareTag("Projectile")) StartCoroutine(Die());
                break;
        }
    }

    GameObject leverColl = null; //현재 작동중인 레버 
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
            leverColl = other.gameObject;
            lever = other.gameObject;
            if (lever.GetComponent<lever>().isPowerLever)
            {
                shouldRotateHalf = true;
            }
        }
        else
        {
            leverColl = null;
        }
        
        switch (GameManager.instance.gameData.curStageNum)
        {
            case 3:
                if (other.CompareTag("RightWind") || other.CompareTag("LeftWind")) isHorizontalWind = true;
                else if (other.CompareTag("UpWind") || other.CompareTag("DownWind")) isVerticalWind = true;
                break;
            case 4:
                if (other.CompareTag("Ghost") && other.GetComponent<SpriteRenderer>().color.a != 0f) StartCoroutine(Die());
                break;
            case 6:
                if (other.CompareTag("Fire")) StartCoroutine(Die());
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
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
        isDieCorWork = true; //이미 코루틴이 실행하고 있는동안은 다음 코루틴을 실행시키지 않음 

        GameManager.instance.shouldSpawnSavePoint = true; //죽은 경우는 일단 세이브포인트에서 시작해야 함 
        cameraObj.GetComponent<MainCamera>().isCameraLock = true; //죽은 경우 카메라는 고정
        cameraObj.GetComponent<MainCamera>().cameraShake(0.5f, 0.7f);
        sprite.color = new Color(1, 1, 1, 0); 

        for(int index=0; index<parts.Length; index++)
        {
            parts[index].SetActive(true);
            Rigidbody2D rigid = parts[index].GetComponent<Rigidbody2D>();

            Vector2 randomDir = new Vector2(Random.insideUnitSphere.x, Random.insideUnitSphere.y);
            float randomPower = Random.Range(20f, 30f);

            rigid.AddForce(randomDir * randomPower, ForceMode2D.Impulse); //플레이어 죽고 나면 각 파편들 랜덤위치로 튐
        }
        yield return new WaitForSeconds(1.5f);

        UIManager.instance.FadeOut(1f); //화면 어두워짐 
        yield return new WaitForSeconds(2f);

        isDieCorWork = false;

        if (GameManager.instance.gameData.curAchievementNum == 0) 
            //만약 스테이지 시작하고 첫 세이브 활성화전에 죽었다면 ~> 그냥 첫 세이브에서 부활 
        {
            GameManager.instance.gameData.curAchievementNum = 1;

            if(GameManager.instance.gameData.finalStageNum == GameManager.instance.gameData.curStageNum)
            {
                if (GameManager.instance.gameData.finalAchievementNum == GameManager.instance.gameData.curAchievementNum)
                {
                    GameManager.instance.gameData.finalAchievementNum = 1; //필요하다면 final 진행도 역시 갱신해 줌 
                }
            }     

            GameManager.instance.gameData.savePointUnlock[GameManager.instance.saveNumCalculate(new Vector2(GameManager.instance.gameData.curStageNum, 1))] = 1;
            GameManager.instance.gameData.respawnScene = SceneManager.GetActiveScene().buildIndex;

            //GameData 저장해 줌 
            string ToJsonData = JsonUtility.ToJson(GameManager.instance.gameData);
            string filePath = Application.persistentDataPath + GameManager.instance.gameDataFileNames[GameManager.instance.curSaveFileNum];
            File.WriteAllText(filePath, ToJsonData);

            GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
            SceneManager.LoadScene(GameManager.instance.nextScene);
        }

        //GameData 의 정보 GM 에 가져와서 초기화 
        GameManager.instance.nextScene = GameManager.instance.gameData.respawnScene;
        GameManager.instance.nextPos = GameManager.instance.gameData.respawnPos;
        GameManager.instance.nextGravityDir = GameManager.instance.gameData.respawnGravityDir;
        GameManager.instance.nextState = States.Walk;

        SceneManager.LoadScene(GameManager.instance.nextScene);
    }

    //현재 플레이어의 애니메이션 
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

    float jumpAniThreshold = 3f;
    void AnimationManager() //플레이어 애니메이션 전체 관리
    {
        if(fsm.State == States.Walk ||  fsm.State == States.Grab) // Walk, Grab ~> 속도에 따라서 walk / idle 애니메이션 할당 
        {
            //바라보는 방향에 따라 flipX 바꿔줌 
            if (InputManager.instance.horizontal == 1) sprite.flipX = false;
            else if (InputManager.instance.horizontal == -1) sprite.flipX = true;

            if (InputManager.instance.horizontal == 0)
            {
                //만약 idle 상태인데 두 발이 모두 땅에 닿아 있다면 new_idle 실행 
                if(rayPosHit_left.collider != null && rayPosHit_right.collider != null)
                {
                    changeAnimation("new_idle");
                }
                
                //만약 idle 상태인데 한 발만 땅에 닿아 있다면 new_cliff 실행
                if((rayPosHit_left.collider == null && rayPosHit_right.collider != null) || (rayPosHit_left.collider != null && rayPosHit_right.collider == null))
                {
                    changeAnimation("new_cliff");
                }              
            }
            else
            {
                changeAnimation("new_walk");               
            }
        }else if(fsm.State == States.AccessLever)
        {
            //달리는 도중에 레버를 작동시키면 달리기 애니메이션 멈춰야 함 
            changeAnimation("new_idle");
        }

        else if(fsm.State == States.Jump || fsm.State == States.Fall) // jump, fall ~> �÷��̾ ���߿� �� ���� �� 
        {
            if (InputManager.instance.horizontal == 1) sprite.flipX = false;
            else if (InputManager.instance.horizontal == -1) sprite.flipX = true;

            float jumpVel = Vector3.Dot(rigid.velocity, transform.up); //+ �̸� �÷��̾ local Y �������� + �ӵ�, - �̸� �÷��̾ local Y �������� - �ӵ� 
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
        else if (fsm.State == States.AccessRope || fsm.State == States.MoveOnRope) //로프에 매달릴 때 
        {
            if (InputManager.instance.horizontal == 1) sprite.flipX = false;
            else if (InputManager.instance.horizontal == -1) sprite.flipX = true;
            
            if (Physics2D.gravity.normalized == Vector2.left) //머리가 오른쪽을 보고 있을 때
            {
                if (rope.CompareTag("VerticalRope")) //세로 로프
                {
                    if (InputManager.instance.horizontal == 0) changeAnimation("new_rope_h_idle");
                    else changeAnimation("new_rope_h_move");
                }
                else //가로 로프 
                {
                    if (InputManager.instance.vertical == 0) changeAnimation("new_rope_v_idle");
                    else changeAnimation("new_rope_v_move");
                }
            }
            else if (Physics2D.gravity.normalized == Vector2.right) //머리가 왼쪽을 보고 있을 때 
            {
                if (rope.CompareTag("VerticalRope")) //세로 로프
                {
                    if (InputManager.instance.horizontal == 0) changeAnimation("new_rope_h_idle");
                    else changeAnimation("new_rope_h_move");
                }
                else //가로 로프 
                {
                    if (InputManager.instance.vertical == 0) changeAnimation("new_rope_v_idle");
                    else changeAnimation("new_rope_v_move");
                }
            }
            else if (Physics2D.gravity.normalized == Vector2.up) //머리가 아래쪽을 보고 있을 때
            {
                if (rope.CompareTag("VerticalRope")) //세로 로프 
                {
                    if (InputManager.instance.vertical == 0) changeAnimation("new_rope_v_idle");
                    else changeAnimation("new_rope_v_move");
                }
                else //가로 로프 
                {
                    if (InputManager.instance.horizontal == 0) changeAnimation("new_rope_h_idle");
                    else changeAnimation("new_rope_h_move");
                }
            }
            else //머리가 위쪽을 보고 있을 때  
            {
                if (rope.CompareTag("VerticalRope")) //세로 로프 
                {
                    if (InputManager.instance.vertical == 0) changeAnimation("new_rope_v_idle");
                    else changeAnimation("new_rope_v_move");
                }
                else //가로 로프 
                {
                    if (InputManager.instance.horizontal == 0) changeAnimation("new_rope_h_idle");
                    else changeAnimation("new_rope_h_move");
                }
            }
        }        
    }

    void HorizontalMove()
    {        
        // 플레이어 좌우움직임 구현 
        int stageNum = GameManager.instance.gameData.curStageNum;
        if (stageNum == 3 && (isHorizontalWind && Physics2D.gravity.x == 0f || isVerticalWind && Physics2D.gravity.y == 0f))
        {
            // Stage3에서 바람이 부는 방향으로는 플레이어가 움직이지 않음 
            return;
        }

        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);

        if (stageNum == 6 && rayHitIcePlatform.collider != null && InputManager.instance.horizontal == 0f)
        { 
            locVel = new Vector2(Vector2.Lerp(locVel, Vector2.zero, Time.deltaTime * slidingDegree).x , locVel.y);
        }
        else if (stageNum == 3 && isPlayerExitFromWInd)
        {
            return;
        }

        else 
        {
            locVel = new Vector2(InputManager.instance.horizontal * walkSpeed, locVel.y);
        }
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void LimitFallingSpeed()
    {
        // 플레이어 떨어질 때 속도제한 
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        if (locVel.y < -maxFallingSpeed) locVel.y = -maxFallingSpeed;
        rigid.velocity = transform.TransformDirection(locVel);
    }

    //플레이어의 오른발, 왼발 ray 체크를 다르게 함 
    RaycastHit2D rayPosHit_left; //왼발
    RaycastHit2D rayPosHit_right; //오른발  

    void CheckGround()
    {        
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
        Vector2 middlePos = transform.position - transform.up; //플레이어 왼발, 오른발 사이 중심 좌표 
        float centerToLeg = 0.3125f;

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2:
                // Platform, Launcher, Stone
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 6 | 1 << 15);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
            case 5:
                // Platform, JumpBoost
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                rayHitJumpBoost = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 22);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                isOnJumpPlatform = rayHitJumpBoost.collider != null;   
                break;
            case 6:
                // Platform, IcePlatform
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                rayHitIcePlatform = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 9);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null || rayHitIcePlatform.collider;
                break;
            case 7:
                // Platform, Enemy
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 18);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
            default:
                // Platform
                //rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.875f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                
                if (transform.up == new Vector3(0, 1, 0)) 
                {
                    rayStartPos_left = middlePos + new Vector2(-centerToLeg, 0);
                    rayStartPos_right = middlePos + new Vector2(centerToLeg, 0);
                }
                else if(transform.up == new Vector3(1, 0, 0))
                {
                    rayStartPos_left = middlePos + new Vector2(0, centerToLeg);
                    rayStartPos_right = middlePos + new Vector2(0, -centerToLeg);
                }
                else if(transform.up == new Vector3(0, -1, 0)) 
                {
                    rayStartPos_left = middlePos + new Vector2(centerToLeg, 0);
                    rayStartPos_right = middlePos + new Vector2(-centerToLeg, 0);
                }
                else 
                {
                    rayStartPos_left = middlePos + new Vector2(0, -centerToLeg);
                    rayStartPos_right = middlePos + new Vector2(0, centerToLeg);
                }

                rayPosHit_left = Physics2D.Raycast(rayStartPos_left, -transform.up, 0.05f, LayerMask.GetMask("Platform"));
                rayPosHit_right = Physics2D.Raycast(rayStartPos_right, -transform.up, 0.05f, LayerMask.GetMask("Platform"));

                //Debug.DrawRay(rayStartPos_left, -transform.up * 0.2f, new Color(1, 0, 0));
                //Debug.DrawRay(rayStartPos_right, -transform.up * 0.2f, new Color(1, 0, 0));

                isGrounded = rayPosHit_left.collider != null || rayPosHit_right.collider != null;

                break;
        }
    }

    // �÷��̾� ���� Vertical (���� ��ǥ ���� X)

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

    // �÷��̾� ���� Horizontal (���� ��ǥ ���� X)
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
        
        // �̵� ���⿡ ���� Player sprite �¿� flip
        if (InputManager.instance.horizontal == 1) sprite.flipX = false;
        else if (InputManager.instance.horizontal == -1) sprite.flipX = true;
    }    
    */

    /************* �Ʒ��� Stage8 �ڵ� ****************/
    /****** Stage8 ��� ������ �� �ڵ� ���� �ʿ� ******/

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