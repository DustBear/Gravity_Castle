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
    [SerializeField] float jumpChargeSpeed; // ���� ������ ���� �ӵ�
    [SerializeField] float ropeAccessSpeed; // rope�� �����ϴ� �ӵ�
    [SerializeField] float ropeMoveSpeed; // rope�� �Ŵ޷� �����̴� �ӵ�
    [SerializeField] float leverMoveSpeed; // lever �۵� �� �÷��̾ �̵��ϴ� �ӵ�
    [SerializeField] float leverRotateSpeed; // lever �۵� �� �÷��̾ ȸ���ϴ� �ӵ�
    [SerializeField] float windForce; // Stage3�� �ٶ��� ���� �޴� ��
    [SerializeField] float slidingDegree; // Stage6�� ���� ������ �̲������� ����

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
    Func<bool> readyToFall, readyToLand, readyToJump, readyToPowerJump, readyToRope, readyToLever; // �� ���·� �̵��ϱ� ���� �⺻ ����

    // Jump
    [SerializeField]float jumpGauge;

    // Rope
    bool isCollideRope;
    GameObject rope; // �Ŵ޸� rope

    // Lever
    bool isCollideLever;
    GameObject lever; // �۵���ų lever
    Vector3 destPos_afterLevering; // Lever �۵� �� �÷��̾� position
    float destRot; // Lever �۵� �� �÷��̾� z-rotation

    // Wind
    bool isHorizontalWind;
    bool isVerticalWind;

    // �÷��̾� �Ʒ��� �ִ� �÷��� ����
    [SerializeField]bool isGrounded;
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
    bool isBlackHoleFalling;

    //����� �ҽ� 
    AudioSource sound;
    [SerializeField] AudioClip moveSound; //�ȱ� or ������ Ż �� ���� �Ҹ�
    [SerializeField] AudioClip jump_landSound; //���� �� ������ �� ���� �Ҹ� 

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

        // �� State�� �Ѿ�� ���� �⺻ ����
        readyToFall = () => (!isGrounded)&&(!isOnJumpPlatform); //���̳� ������ȭ���� �� �ٿ� ������� ���� �� 
        readyToLand = () => (isGrounded || isOnJumpPlatform) && (int)transform.InverseTransformDirection(rigid.velocity).y <= 0; //��or������ȭ���ǿ� ��� �ְ� y���� �ӵ������� ������ -1�� �� 
        readyToJump = () => InputManager.instance.jumpDown && (!isOnJumpPlatform); //�����̽��ٸ� ������ ��ȭ���� ���� ���� ���� ���� �� 
        readyToPowerJump = () =>  InputManager.instance.jumpUp && (isOnJumpPlatform); //��ȭ���� ���� ������ �����̽��ٸ� ������ �� ��
        readyToRope = () => isCollideRope && InputManager.instance.vertical == 1; //���� ȭ��ǥ ������ �ְ� + ������ ������� ��
        readyToLever = () => isCollideLever && InputManager.instance.vertical == 1 && InputManager.instance.verticalDown 
                            && lever.transform.eulerAngles.z == transform.eulerAngles.z;
                            //������ ��� �ְ�, ������ ������ rotation�� ������ �ְ�, ���� ȭ��ǥ�� ������ ���� �� 

        // Scene�� ���̺� ����Ʈ���� �������� ���� �� �÷��̾� ������ ����
        // ���� Scene���� �Ѿ���� ������ �����͸� �ҷ��ͼ� ����
        if (!GameManager.instance.shouldStartAtSavePoint)
        {
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
            if (GameManager.instance.gameData.curStageNum == 8 && GameManager.instance.gameData.curAchievementNum == 33 && !GameManager.instance.isCliffChecked)
            {
                InputManager.instance.isInputBlocked = true;
            }
        }

        //�� ������ �� �÷��̾��� ��ġ�� scene Elevator�� �Ѱ���(openingSceneElevator �� �۵����� ���� ����)
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

    public float defaultJumpSpeed;
    public float defaultJumpDelay; //�Ϲ� �������� �ְ���̿� �����ϴ� �� �ɸ��� �ð�
    public float maxJumpHeight; //�Ϲ� ������ ������ �� �ִ� �ִ� ����
    [SerializeField] float defaultJumpTimer;
    [SerializeField] bool shouldAddJumpForce = false; //true �� ���� ���������� �÷��̾ jumpForce�� ���Ѵ� 

    float jumpUpdateForce(float timer) => -2 * (maxJumpHeight / defaultJumpDelay * defaultJumpDelay) * timer + 2 * (maxJumpHeight / defaultJumpDelay * defaultJumpDelay);
    //ü���ð��� ���� �÷��̾� ��¼ӵ� ��� 

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

    void Jump_Enter()
    {
        rigid.velocity = Vector2.zero; // �ٴ� �÷����� �ӵ��� ���� �ӵ��� ������ ��ġ�� ���� ����    
        StartCoroutine("jumpAddForce");
        
        defaultJumpTimer = 0; //Ÿ�̸� 0���� 
        shouldAddJumpForce = true;
        ani.SetBool("isJumping", true);

        sound.clip = jump_landSound;
        sound.Play();
    }

    IEnumerator jumpAddForce()
    {
        int jumpIndex = 0;

        while (jumpIndex < 50) //������ ü���� ���� ���� Update������ ª�� ���� ������ ���������� ����
        {
            Vector2 addForceDirection;
            if (transform.up == new Vector3(0, 1, 0)) //�÷��̾ ������ ���� �� ���� �� 
            {
                addForceDirection = new Vector2(rigid.velocity.x, defaultJumpSpeed);
            }
            else if (transform.up == new Vector3(0, -1, 0)) //�÷��̾ �Ʒ����� ���� �� ���� �� 
            {
                addForceDirection = new Vector2(rigid.velocity.x, -defaultJumpSpeed);
            }
            else if (transform.up == new Vector3(1, 0, 0)) //�÷��̾ �������� ���� �� ���� �� 
            {
                addForceDirection = new Vector2(defaultJumpSpeed, rigid.velocity.y);
            }
            else //�÷��̾ ������ ���� �� ���� ��
            {
                addForceDirection = new Vector2(-defaultJumpSpeed, rigid.velocity.y);
            }

            rigid.velocity = addForceDirection;
            defaultJumpTimer += Time.deltaTime;

            //rigid.velocity = addForceDirection * defaultJumpSpeed * Time.deltaTime; 
            yield return new WaitForSeconds(defaultJumpDelay / 50);

            if (defaultJumpTimer >= defaultJumpDelay || InputManager.instance.jumpUp) //�����̽��ٸ� ���ų� Ÿ�̸Ӱ� ������ ���� �ߴ�
            {
                shouldAddJumpForce = false;
                defaultJumpTimer = 0; //Ÿ�̸� 0���� 
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
            //rigid.velocity = new Vector2(rigid.velocity.x, jumpUpdateForce(defaultJumpTimer)); //�����̽��ٸ� ������ �ִ� ���� ��� �� ����
        }
        
        /*
        if(defaultJumpTimer>=defaultJumpDelay || InputManager.instance.jumpUp) //�����̽��ٸ� ���ų� Ÿ�̸Ӱ� ������ ���� �ߴ�
        {
            shouldAddJumpForce = false;
            defaultJumpTimer = 0; //Ÿ�̸� 0���� 
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
        rigid.velocity = Vector2.zero; // �ٴ� �÷����� �ӵ��� ���� �ӵ��� ������ ��ġ�� ���� ����
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
        else if (ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f) // Land �ִϸ��̼��� ������ Walk State�� ��ȯ
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

        // rope�� �Ŵ޸��� �ִϸ��̼� ����
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

        // Rope�� �Ŵ޸� ���·� �̵�
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
        ani.SetBool("isRopingVerticalIdle", false);
        ani.SetBool("isRopingHorizontalIdle", false);
        ani.SetBool("isRopingVerticalMove", false);
        ani.SetBool("isRopingHorizontalMove", false);
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
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
    }

    void ChangeGravityDir_Enter()
    {
        rigid.gravityScale = 0f;
        rigid.velocity = Vector2.zero;

        // �÷��̾��� �θ� ������Ʈ�� �ִٸ� ����. �׷��� ������ �÷��̾�� �Բ� ȸ����
        transform.parent = null;
        
        // ȸ���ؾ��� �÷��̾� rotation ����
        destRot = transform.eulerAngles.z + InputManager.instance.horizontal * 90f;
        if (destRot == -90f) destRot = 270f;

        // ȸ���ϸ鼭 �ٲ����� �÷��̾� position ����
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
        // ���� ���� �� �÷��̾� �̵� �� ȸ��
        transform.localPosition = Vector2.MoveTowards(transform.localPosition, destPos_afterLevering, leverMoveSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(Vector3.forward * destRot), leverRotateSpeed * Time.deltaTime);
        
        // �÷��̾� ȸ���� ���� �Ϸ�Ǿ��ٸ� ���� state�� �̵�
        if (Mathf.RoundToInt(transform.eulerAngles.z) == destRot)
        {
            // �÷��̾� ������ ȸ��
            if (destRot == 360f) destRot = 0f;
            transform.eulerAngles = Vector3.forward * destRot;

            // �߷� ���� ��ȭ
            Vector2 gravity = -transform.up * 9.8f;
            if (Mathf.Abs(gravity.x) < 1f) gravity.x = 0f;
            else gravity.y = 0f;
            Physics2D.gravity = gravity;

            // �÷��̾�� �߷� ����
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
        if (other.gameObject.CompareTag("Spike")) Die();

        switch (GameManager.instance.gameData.curStageNum)
        {
            case 2: case 6: case 7:
                // �������� 2: Cannon, Arrow�� �΋H���� ���
                // �������� 6: �������� Fire�� �΋H���� ���
                // �������� 7: Bullet�� �΋H���� ���
                if (other.gameObject.CompareTag("Projectile")) Die();
                break;
            case 8:
                // �������� 8: Devil, Devil�� ��� �������� �΋H���� ���
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
                // �������� 4 : �������� ���� Ghost�� �΋H���� ���
                if (other.CompareTag("Ghost") && other.GetComponent<SpriteRenderer>().color.a != 0f) Die();
                break;
            case 6:
                // �������� 6 : ���� ���� �ִ� Fire�� �΋H���� ���
                if (other.CompareTag("Fire")) Die();
                break;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // �������� 3���� Rope�� �Ŵ޷� ���� ���� ���·� �ٶ��� ������ ���ư�
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
        GameManager.instance.shouldStartAtSavePoint = true; //������ �ϴ� ���̺�����Ʈ�� �����ؾ� �� 
        UIManager.instance.FadeOut();
    }

    void HorizontalMove()
    {
        // Walk �ִϸ��̼�
        if (InputManager.instance.horizontal != 0)
        {           
            ani.SetBool("isWalking", true);
        }
        else ani.SetBool("isWalking", false);

        // Walk ���⿡ ���� Player sprite �¿� flip
        if (InputManager.instance.horizontal == 1) sprite.flipX = false;
        else if (InputManager.instance.horizontal == -1) sprite.flipX = true;

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
        else
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
                rayHit = Physics2D.BoxCast(transform.position, new Vector2(0.85f, 0.1f), transform.eulerAngles.z, -transform.up, 1f, 1 << 3);
                isGrounded = rayHit.collider != null || rayHitMovingFloor.collider != null;
                break;
        }
    }

    // �÷��̾� ���� Vertical (���� ��ǥ ���� X)
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

    /************* �Ʒ��� Stage8 �ڵ� ****************/
    /****** Stage8 ��� ������ �� �ڵ� ���� �ʿ� ******/
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