using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MonsterLove.StateMachine;

using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{   
    //�÷��̾� ���� �� ���� Ƣ�� �� ���� 
    public GameObject[] parts = new GameObject[4];
    bool isDieCorWork;

    [SerializeField] float defaultGravityScale;
    [SerializeField] float maxFallingSpeed;
    [SerializeField] public float walkSpeed;
    [SerializeField] public float maxWindSpeed; //stage3���� �ٶ��� ���ư� �� �ִ� �ӵ� 
    [SerializeField] float minJumpPower;
    [SerializeField] float maxJumpPower;
    [SerializeField] float maxJumpBoostPower;
    [SerializeField] float jumpChargeSpeed; // ���� ������ ���� �ӵ�
    [SerializeField] float ropeAccessSpeed; // rope�� �����ϴ� �ӵ�
    [SerializeField] float ropeMoveSpeed; // rope�� �Ŵ޷� �����̴� �ӵ�
    [SerializeField] float leverRotateDelay; // lever �۵� �� �÷��̾ ȸ���ϴ� �� �ɸ��� �ð� 
    public float windForce; // Stage3�� �ٶ��� ���� �޴� ��~> ȯǳ�⸶�� �ٸ��� ������ �� �ֵ��� �ܺ����� ��� 
    [SerializeField] float slidingDegree; // Stage6�� ���� ������ �̲������� ����

    public bool isPlayerInSideStage; 
    //�÷��̾ ���̵� �������� ���� ���� ���� �װ� ���� ������ respawnPos���� ���� ��Ȱ

    // �÷����� ���´� Finite State Machine���� ����
    public enum States
    {
        Walk, Fall, Land, Jump, 
        PowerJump, // Stage5 ��ȭ ����
        AccessRope, MoveOnRope,
        AccessLever, SelectGravityDir, ChangeGravityDir, FallAfterLevering,
        GhostUsingLever, FallAfterGhostLevering // Stage4 ����
    }
    StateMachine<States> fsm;
    [HideInInspector] public static States curState {get; private set;}
    Func<bool> readyToFall, readyToLand, readyToJump, readyToPowerJump, readyToRope, readyToLever , readyToPowerLever; // �� ���·� �̵��ϱ� ���� �⺻ ����

    // Walk
    public bool isPlayerExitFromWInd; //�÷��̾ stage3 windZone���� �������� ���� ���� ������ ������ �ް� ���� �� true 

    // Jump
    [SerializeField]float jumpGauge;
    [SerializeField] float jumpTimer; //���� ��� ���� �����̽��ٸ� ������ ���� �������� ���� �ִٸ� ������� �ν��ؾ� �� 
    public float jumpTimerOffset; //������ 
    public float jumpHeightCut; //�����ϴٰ� �����̽��ٿ��� ���� ���� �������� �ӵ��� ����

    [SerializeField] float groundedRemember; //�÷������� ������ ���Ŀ��� �������� �̳� �ð������� ������ �� �־�� ��
    [SerializeField] float groundedRememberOffset;

    // Rope
    bool isCollideRope;
    GameObject rope; // �Ŵ޸� rope

    // Lever
    bool isCollideLever;
    public bool shouldRotateHalf; //powerLever �� �ƴϸ� 90���� ȸ��, powerLever �̸� 180�� ȸ�� 
    GameObject lever; // �۵���ų lever
    Vector3 destPos_afterLevering; // Lever �۵� �� �÷��̾� position
    [SerializeField] int destRot; // Lever �۵� �� �÷��̾ ȸ���ϰ��� �ϴ� ����
    float destGhostRot; //��������4�� ������ ������ ���� �� �ʿ��� ���� 

    // Wind
    bool isHorizontalWind;
    bool isVerticalWind;

    // �÷��̾� �Ʒ��� �ִ� �÷��� ����
    [SerializeField]public bool isGrounded;
    [SerializeField] bool isOnJumpPlatform; //��ȭ���� ���� ���� �ִ��� ����
    RaycastHit2D rayHitIcePlatform;
    RaycastHit2D rayHitJumpBoost;

    [SerializeField] GameObject leftArrow;
    [SerializeField] GameObject rightArrow;
    MainCamera mainCamera;
    Rigidbody2D rigid;
    BoxCollider2D collide;
    SpriteRenderer sprite;
    Animator ani;

    //�� ������ �� ���������Ϳ� ��ȣ�ۿ�
    GameObject openingSceneElevator;

    // Stage8 ����
    /********Stage8 ��� ������ �� �ڵ� ���� �ʿ� **********/
    [HideInInspector] public bool isDevilRotating;
    [HideInInspector] public bool isBlackHole;
    bool isDevilFalling;

    //����� �ҽ� 
    AudioSource sound;
    [SerializeField] AudioClip moveSound; //�ȱ� or ������ Ż �� ���� �Ҹ�
    [SerializeField] AudioClip jump_landSound; //���� �� ������ �� ���� �Ҹ� 

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
        sprite.color = new Color(1, 1, 1, 1); //�÷��̾� ���� �ʱ�ȭ 

        // �� State�� �Ѿ�� ���� �⺻ ����
        readyToFall = () => (!isGrounded)&&(!isOnJumpPlatform); //���̳� ������ȭ���� �� �ٿ� ������� ���� �� 
        readyToLand = () => (isGrounded || isOnJumpPlatform) && (int)transform.InverseTransformDirection(rigid.velocity).y <= 0; //��or������ȭ���ǿ� ��� �ְ� y���� �ӵ������� ������ -1�� �� 

        readyToJump = () => (jumpTimer > 0) && (!isOnJumpPlatform) && (groundedRemember > 0); //�����̽��ٸ� ������ ��ȭ���� ���� ���� ���� ���� �� 

        readyToPowerJump = () =>  InputManager.instance.jumpUp && (isOnJumpPlatform) && (groundedRemember > 0); //��ȭ���� ���� ������ �����̽��ٸ� ������ �� ��
        readyToRope = () => isCollideRope && InputManager.instance.vertical == 1; //���� ȭ��ǥ ������ �ְ� + ������ ������� ��
        readyToLever = () => isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown 
                             && lever.transform.up == transform.up;
                             //������ ��� �ְ�, ������ ������ rotation�� ������ �ְ�, ���� ȭ��ǥ�� ������ ���� �� 

        // Scene�� ���̺� ����Ʈ���� �������� ���� �� �÷��̾� ������ ����
        // ���� Scene���� �Ѿ���� ������ �����͸� �ҷ��ͼ� ����
        if (!GameManager.instance.shouldStartAtSavePoint)
        {
            //GameManager ~> ���� ������ �� �÷��̾��� ��ġ ���� 
            transform.position = GameManager.instance.nextPos;
            Physics2D.gravity = GameManager.instance.nextGravityDir * 9.8f;
            transform.up = -GameManager.instance.nextGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z; // x-rotation, y-rotation�� 0���� ����
            
            States nextState = GameManager.instance.nextState;
            // ���� scene�� rope�� ���� scene�� rope�� �ٸ� ������Ʈ�� ��޵Ǵ� ���� ���� �ʿ�

            if (nextState == States.MoveOnRope) ChangeState(States.AccessRope);
            // Jump state�� �����ϸ� ���� input�� �ȵ��͵� scene�� ���۵��ڸ��� �÷��̾ �����ϴ� ���� �߻�
            else if (nextState == States.Jump) ChangeState(States.Fall);
            else ChangeState(nextState);
        }

        // Scene�� ���̺� ����Ʈ���� ������ �� (����� �� ��Ȱ, ����� ���� �ҷ�����) �÷��̾� ������ ����
        else
        {
            transform.position = GameManager.instance.gameData.respawnPos;
            Physics2D.gravity = GameManager.instance.gameData.respawnGravityDir * 9.8f;
            transform.up = -GameManager.instance.gameData.respawnGravityDir;
            transform.eulerAngles = Vector3.forward * transform.eulerAngles.z; // x-rotation, y-rotation�� 0���� ����
            ChangeState(States.Walk); // ������ �� �÷��̾ �̼��ϰ� ���߿� �� ���� �� �����Ƿ� Fall state�� ����
            GameManager.instance.shouldStartAtSavePoint = false;

            /*********** curAchievementNum �κ��� ��������8 ���̺� ����Ʈ ������ �Ϸ�Ǹ� ���� �ʿ� ***********/
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

        jumpTimer = 0; //jumpTimer �ʱ�ȭ 

        //�÷��̾ 0���� 360 ������ rotation ���� �������� �ʱ�ȭ���� 
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

        jumpTimer -= Time.deltaTime; //jumpTimer �� �����Ӹ��� �۵� 
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
        else if (readyToJump()) //�����̽��ٸ� ������ ���� �۵�
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

    void ChargeJumpGauge() //��������5 ��ȭ���� ��Ϳ����� ��� 
    {
        if (InputManager.instance.jump) //�����̽��� ���� �� 
        {
            // Jump boost ���� ������ ������ ���� 
            if (rayHitJumpBoost.collider != null)
            {
                jumpGauge = Mathf.Clamp(jumpGauge + jumpChargeSpeed * Time.deltaTime, minJumpPower, maxJumpBoostPower);
                Debug.Log("���� ������ ������ �� : " + (jumpGauge - minJumpPower) / (maxJumpBoostPower - minJumpPower) * 100f + "%");
            }            
        }
    }

    public float jumpPower;

    void Jump_Enter()
    {
        jumpTimer = 0;
        rigid.velocity = Vector2.zero; // �ٴ� �÷����� �ӵ��� ���� �ӵ��� ������ ��ġ�� ���� ����    
        
        Vector2 addForceDirection;
        
        if (transform.up == new Vector3(0, 1, 0)) //�÷��̾ ������ ���� �� ���� �� 
        {
            addForceDirection = new Vector2(0, 1);
        }
        else if (transform.up == new Vector3(0, -1, 0)) //�÷��̾ �Ʒ����� ���� �� ���� �� 
        {
            addForceDirection = new Vector2(0, -1);
        }
        else if (transform.up == new Vector3(1, 0, 0)) //�÷��̾ �������� ���� �� ���� �� 
        {
            addForceDirection = new Vector2(1, 0);
        }
        else //�÷��̾ ������ ���� �� ���� ��
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

            if (transform.up == new Vector3(0, 1, 0)) //�÷��̾ ������ ���� �� ���� �� 
            {
                addForceDirection = new Vector2(rigid.velocity.x, jumpPower);
            }
            else if (transform.up == new Vector3(0, -1, 0)) //�÷��̾ �Ʒ����� ���� �� ���� �� 
            {
                addForceDirection = new Vector2(rigid.velocity.x, -jumpPower);
            }
            else if (transform.up == new Vector3(1, 0, 0)) //�÷��̾ �������� ���� �� ���� �� 
            {
                addForceDirection = new Vector2(jumpPower, rigid.velocity.y);
            }
            else //�÷��̾ ������ ���� �� ���� ��
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

        if (InputManager.instance.jumpUp) //�����̽��ٿ��� ���� ���� ���� ���� �ӵ� �ٿ��� �� 
        {
            if (transform.up.normalized == new Vector3(0, 1, 0) && rigid.velocity.y > 0) //�÷��̾ ������ �ٶ󺸰� ���� �� + �ӵ��� �������� ���� ��
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * jumpHeightCut, 0); //�������� �ӵ� ���� 
            }
            else if (transform.up.normalized == new Vector3(0, -1, 0) && rigid.velocity.y < 0) //�÷��̾ �Ʒ����� �ٶ󺸰� ���� �� 
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y * jumpHeightCut, 0);
            }
            else if (transform.up.normalized == new Vector3(1, 0, 0) && rigid.velocity.x > 0) //�÷��̾ �������� �ٶ󺸰� ���� ��
            {
                rigid.velocity = new Vector3(rigid.velocity.x * jumpHeightCut, rigid.velocity.y, 0);
            }
            else if (transform.up.normalized == new Vector3(-1, 0, 0) && rigid.velocity.x < 0)//�÷��̾ ������ �ٶ󺸰� ���� �� 
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
        rigid.velocity = Vector2.zero; // �ٴ� �÷����� �ӵ��� ���� �ӵ��� ������ ��ġ�� ���� ����
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
            ChangeState(States.Land); //������ �� �ִ� �����̸� �ٷ� Walk ���·� ��ȯ 
        }else if (readyToJump())
        {
            ChangeState(States.Jump); //�������� ��������� �÷������� �������� ������ ���Ŀ��� ���� ����
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
        else // Land �ִϸ��̼��� ������ Walk State�� ��ȯ
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
        // ���� scene���� rope�� �Ŵ޸� ���·� ���� scene���� �Ѿ�Դٸ� rope�� ���� �ν��ؾ���
        if (rope == null) return;

        // �Ŵ޸� ��ġ ����
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
        if (Vector2.Distance(transform.position, destPos_rope) < 0.1f) // �÷��̾ rope�� �������� ��
        {
            // �÷��̾ rope�� ������ �̵���Ŵ
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

        // Rope�� �Ŵ޸� ���·� �̵�
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

        // Rope���� ����
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
       
        // ���� �������� �÷��̾� sprite flip
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

        // ������ ������ ���� �÷��̾ �̵��ؾ��� position ����
        switch (lever.transform.eulerAngles.z)
        {
            case 0f: case 180f:
                destPos_beforeLevering = new Vector2(lever.transform.position.x, transform.position.y);
                break;

            default:
                destPos_beforeLevering = new Vector2(transform.position.x, lever.transform.position.y);
                break;
        }

        // �̵�
        float moveToLeverSpeed = 9f;
        transform.position = Vector2.MoveTowards(transform.position, destPos_beforeLevering, moveToLeverSpeed * Time.deltaTime);        

        if ((Vector2)transform.position == destPos_beforeLevering)
        {
            ChangeState(States.SelectGravityDir);
        }
    }
   
    void SelectGravityDir_Enter()
    {
        if (!shouldRotateHalf) //180�� ȸ���ϴ� ��ȭ������ ��� ȭ��ǥ�� ����� ���� 
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }       
    }

    void SelectGravityDir_Update()
    {
        // ���� ����
        if (InputManager.instance.horizontalDown)
        {
            ChangeState(States.ChangeGravityDir);
        }
        // ���� ������ ���
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

        // �÷��̾��� �θ� ������Ʈ�� �ִٸ� ����. �׷��� ������ �÷��̾�� �Բ� ȸ����
        transform.parent = null;

        // ȸ���ؾ��� �÷��̾� rotation ����
        if (!shouldRotateHalf) //90�� ȸ���ϴ� �Ϲ����� ��� 
        {           
            if(InputManager.instance.horizontal == 1) //z rotation + 90 degree (�ݽð���� ȸ��)
            {
                destRot = 90;
            }
            else if (InputManager.instance.horizontal == -1)
            {
                destRot = -90;
            }
        }
        else //180�� ȸ���ϴ� ��� 
        {
            if (!sprite.flipX)
            {
                destRot = 180;
            }
            else
            {
                destRot = -180;
            }
            //180�� ȸ�� ������ ��� �÷��̾ �ٶ󺸴� ���⿡ ���� ȸ�������� �޶����� 
        }

        // ȸ���ϸ鼭 �ٲ����� �÷��̾� position ����
        if (!shouldRotateHalf) //90��ȸ����
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
        else //180�� ȸ���� 
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
        initZRot = Mathf.RoundToInt(transform.eulerAngles.z); //�������� ��������� ���� ���׸� ���� ���� int ������ �ݿø� 

        Time.timeScale = 0; //�÷��̾ ȸ���ϴµ��� �ð� ���� 

        cameraObj.GetComponent<MainCamera>().cameraShake(0.3f, 0.5f);
    }

    float timer=0;
    float initZRot;
    void ChangeGravityDir_Update()
    {
        if (isCameraShake) return; //ī�޶� ��鸲�� ������ ���� �÷��̾� ȸ�� 

        // ���� ���� �� �÷��̾� �̵� �� ȸ��
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

        // �߷� ���� ��ȭ
        Vector2 gravity = -transform.up * 9.8f;
        if (Mathf.Abs(gravity.x) < 1f) gravity.x = 0f; //�������� ������� ����(0.00xxx ���� ���� ��� 0���� ��������� ��) 
        else gravity.y = 0f;

        Physics2D.gravity = gravity; //�� ��ü �߷¹��� �ٲ� 

        // �÷��̾�� �߷� ����
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

        // �÷��̾� rotation�� ���� (0, 0, 0)�̸� ���� state�� �̵�
        int angle = Mathf.RoundToInt(transform.eulerAngles.z);
        if (angle == 0 || angle == 360 )
        {
            ChangeState(States.FallAfterGhostLevering);    
        }
    }

    void FallAfterGhostLevering_Enter()
    {
        // �÷��̾� ������ ȸ��
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
        // ��� ������������ Spike�� �΋H���� ���
        if (other.gameObject.CompareTag("Spike") && !isDieCorWork)
        {
            StartCoroutine(Die());
        }

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2: case 6: case 7:
                // �������� 2: Cannon, Arrow�� �΋H���� ���
                // �������� 6: �������� Fire�� �΋H���� ���
                // �������� 7: Bullet�� �΋H���� ���
                if (other.gameObject.CompareTag("Projectile") && !isDieCorWork)
                {
                    StartCoroutine(Die());
                }
                break;
            case 8:
                // �������� 8: Devil, Devil�� ��� �������� �΋H���� ���
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
                // �������� 4 : �������� ���� Ghost�� �΋H���� ���
                if (other.CompareTag("Ghost") && other.GetComponent<SpriteRenderer>().color.a != 0f) StartCoroutine(Die());
                break;
            case 6:
                // �������� 6 : ���� ���� �ִ� Fire�� �΋H���� ���
                if (other.CompareTag("Fire")) StartCoroutine(Die());
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // �������� 3���� Rope�� �Ŵ޷� ���� ���� ���·� �ٶ��� ������ ���ư�
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
        isDieCorWork = true; //���� ����� �������� �� Ʈ���Ű� �ߵ��ϴ��� �ٽ� ���� ���� 

        GameManager.instance.shouldStartAtSavePoint = true; //������ �ϴ� ���̺�����Ʈ���� �����ؾ� �� 
        cameraObj.GetComponent<MainCamera>().isCameraLock = true; //ī�޶� �������� �ʰ� ����
        cameraObj.GetComponent<MainCamera>().cameraShake(0.5f, 0.7f);
        sprite.color = new Color(1, 1, 1, 0); //��� �÷��̾� ����ȭ 

        for(int index=0; index<parts.Length; index++)
        {
            parts[index].SetActive(true);
            Rigidbody2D rigid = parts[index].GetComponent<Rigidbody2D>();

            Vector2 randomDir = new Vector2(Random.insideUnitSphere.x, Random.insideUnitSphere.y);
            float randomPower = Random.Range(20f, 30f);

            rigid.AddForce(randomDir * randomPower, ForceMode2D.Impulse); //�� ������ ���� ����,ũ���� ���� ���ؼ� ƨ�ܳ� 
        }
        yield return new WaitForSeconds(1.5f);

        UIManager.instance.FadeOut(1f); //ȭ�� ��ο�����
        yield return new WaitForSeconds(2f);

        isDieCorWork = false;
        GameManager.instance.StartGame(false); //���� ����� ~> �ʱ�ȭ �� initData(false) �� ���� 
    }

    //�ִϸ��̼� ��ȯ 
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
    void AnimationManager() //�÷��̾� ���� ��ü�� �ִϸ��̼� ���� 
    {
        if(fsm.State == States.Walk) // Walk, idle �ִϸ��̼� ~> �÷��̾ ���鿡 ������� �� 
        {
            // Walk ���⿡ ���� Player sprite �¿� flip
            if (InputManager.instance.horizontal == 1) sprite.flipX = false;
            else if (InputManager.instance.horizontal == -1) sprite.flipX = true;

            if (InputManager.instance.horizontal == 0)
            {
                //�÷��̾��� �� ���� ��� ���� ������� �� 
                if(rayPosHit_left.collider != null && rayPosHit_right.collider != null)
                {
                    changeAnimation("new_idle");
                }
                
                //�÷��̾��� �� �� �� �ϳ��� ���� ������� �� 
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
        else if(fsm.State == States.Jump || fsm.State == States.Fall) // jump, fall ~> �÷��̾ ���߿� �� ���� �� 
        {
            // Walk ���⿡ ���� Player sprite �¿� flip
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
        else if (fsm.State == States.AccessRope || fsm.State == States.MoveOnRope)
        {
            if (rope.CompareTag("VerticalRope")) //���� ������ �� 
            {
                if(InputManager.instance.vertical == 0)
                {
                    changeAnimation("new_ropeVertical"); //���� ���� ���������� 
                }
                else
                {
                    changeAnimation("new_ropeVertical_move"); //���� ������ ������ 
                }
            }
            else if (rope.CompareTag("HorizontalRope")) //���� ������ �� 
            {
                if(InputManager.instance.horizontal == 0)
                {
                    changeAnimation("new_ropeHorizontal"); //���� ���� ���������� 
                }
                else
                {
                    changeAnimation("new_ropeHorizontal_move"); //���� ������ ������ 
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
        // �̵� ����
        int stageNum = GameManager.instance.gameData.curStageNum;
        if (stageNum == 3 && (isHorizontalWind && Physics2D.gravity.x == 0f || isVerticalWind && Physics2D.gravity.y == 0f))
        {
            // Stage3���� �Ҿ���� �ٶ��� ���� �����δ� �÷��̾ Ű�� ������ �̵� �Ұ�.
            return;
        }

        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);

        if (stageNum == 6 && rayHitIcePlatform.collider != null && InputManager.instance.horizontal == 0f)
        {
            // Stage6���� ���� ����� Ű���� input�� ���� �Ŀ� �̲�����
            locVel = new Vector2(Vector2.Lerp(locVel, Vector2.zero, Time.deltaTime * slidingDegree).x , locVel.y);
        }
        else if (stageNum == 3 && isPlayerExitFromWInd)
        {
            //stage03 ���� ȯǳ�� �ٶ� ������ �ް� ���� ���� �������� windPower ��ũ��Ʈ���� ���� 
            return;
        }

        else //�Ϲ����� ���̽� 
        {
            // ���� ���� �ƴ϶�� Ű���� input�� ���� ������ �̵�
            locVel = new Vector2(InputManager.instance.horizontal * walkSpeed, locVel.y);
        }
        rigid.velocity = transform.TransformDirection(locVel);
    }

    void LimitFallingSpeed()
    {
        // ������ �� �ִ� �ӵ� ����
        Vector2 locVel = transform.InverseTransformDirection(rigid.velocity);
        if (locVel.y < -maxFallingSpeed) locVel.y = -maxFallingSpeed;
        rigid.velocity = transform.TransformDirection(locVel);
    }

    //�÷��̾ �÷��� ���� �� �߷� �� �������� ��� ������ ���� hit 
    RaycastHit2D rayPosHit_left; //���� 
    RaycastHit2D rayPosHit_right; //������ 

    void CheckGround()
    {
        // �÷��̾ �÷����� �Բ� �������� �ϴ� ��Ȳ
        // 1) �����̴� �÷��� ���� ���� ���. ��, �����̴� �÷��� ������ ������ �۵���ų ���� �Բ� �����̸� �ȵ�
        // 2) �����̴� �÷����� ����� rope�� �Ŵ޷� �ִ� ���

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
        Vector2 middlePos = transform.position - transform.up; //(x,y) �� ��ǥ�� ����� 
        float centerToLeg = 0.3125f;

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2:
                // Platform, Launcher, Stone ����
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 6 | 1 << 15);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
            case 5:
                // Platform, JumpBoost ����
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                rayHitJumpBoost = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 22);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                isOnJumpPlatform = rayHitJumpBoost.collider != null; //���� ��ȭ ���� ���� �ִ� �Ͱ� �Ϲ� �÷��� ���� �ִ� �� ����                
                break;
            case 6:
                // Platform, IcePlatform ����
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                rayHitIcePlatform = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 9);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null || rayHitIcePlatform.collider;
                break;
            case 7:
                // Platform, Enemy ����
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3 | 1 << 18);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
            default:
                // Platform ����
                //rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.875f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);

                isGrounded = rayPosHit_left.collider != null || rayPosHit_right.collider != null;
              
                if (transform.up == new Vector3(0, 1, 0)) // �Ӹ��� ������ ���� 
                {
                    rayStartPos_left = middlePos + new Vector2(-centerToLeg, 0);
                    rayStartPos_right = middlePos + new Vector2(centerToLeg, 0);
                }
                else if(transform.up == new Vector3(1, 0, 0)) // �Ӹ��� �������� ����
                {
                    rayStartPos_left = middlePos + new Vector2(0, centerToLeg);
                    rayStartPos_right = middlePos + new Vector2(0, -centerToLeg);
                }
                else if(transform.up == new Vector3(0, -1, 0)) //�Ӹ��� �Ʒ����� ���� 
                {
                    rayStartPos_left = middlePos + new Vector2(centerToLeg, 0);
                    rayStartPos_right = middlePos + new Vector2(-centerToLeg, 0);
                }
                else //�Ӹ��� ������ ���� 
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