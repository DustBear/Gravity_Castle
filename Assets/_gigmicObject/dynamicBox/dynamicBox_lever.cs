using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dynamicBox_lever : MonoBehaviour
{    
    GameObject playerObj;
    GameObject playerBoxColl;
    Player playerscr;
    SpriteRenderer spr;

    GameObject dynamicBox;
    Rigidbody2D rigid;

    bool isLeverOn;
    [SerializeField] bool isBoxGrab;
    [SerializeField] bool isPlayerOn;

    public float grabThreshold; //이 거리 이상 box와 플레이어가 멀어지면 grab 이 해제된다 

    public float grabSpeed;
    float initWalkSpeed;
    float boxOffset;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();
        dynamicBox = transform.parent.gameObject;
        rigid = dynamicBox.GetComponent<Rigidbody2D>();
        playerObj = GameObject.FindWithTag("Player");
        playerscr = playerObj.GetComponent<Player>();
        playerBoxColl = playerObj.GetComponent<Player>().boxGrabColl;
    }
    void Start()
    {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        initWalkSpeed = playerscr.walkSpeed;
        isPlayerOn = false;
        isBoxGrab = false;
    }

    void Update()
    {
        leverRay();

        if(Input.GetKeyDown(KeyCode.E)) //E 를 눌렀을 때 
        {
            if (isPlayerOn && !isBoxGrab) //플레이어가 센서 내에 있고 아직 box를 잡고 있지 않으면 
            {
                isBoxGrab = true;
                playerscr.isPlayerGrab = true;
                playerscr.walkSpeed = grabSpeed; //box 들고있으면 속도 느려짐 

                StartCoroutine(boxGrab());
            }           

            else if (isBoxGrab)
            {
                isBoxGrab = false; //상자 내려놓기 
                playerscr.isPlayerGrab = false;
                playerscr.walkSpeed = initWalkSpeed;
            }
        }

        if (isBoxGrab)
        {
            if((dynamicBox.transform.position - playerObj.transform.position).magnitude >= grabThreshold)
            {
                isBoxGrab = false;
                playerscr.isPlayerGrab = false;
                playerscr.walkSpeed = initWalkSpeed;

                rigid.constraints = RigidbodyConstraints2D.None;
                dynamicBox.GetComponent<BoxCollider2D>().isTrigger = false; //다시 콜라이더 킴 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("platform detected");
        if(collision.tag == "Platform")
        {

        }
    }

    Vector3 initDistance;
    
    IEnumerator boxGrab()
    {
        initDistance = dynamicBox.transform.position - playerObj.transform.position;
        if(playerObj.transform.up == new Vector3(0,1,0) || playerObj.transform.up == new Vector3(0, -1, 0)) //플레이어가 0, 180도 쪽으로 머리를 할 때 
        {
            initDistance = new Vector3(initDistance.x, 0, 0);
        }
        else //플레이어가 90, 270도 쪽으로 머리를 할 때 
        {
            initDistance = new Vector3(0, initDistance.y, 0);
        }

        initDistance = initDistance.normalized * 0.5f * flipOffset;
       
        dynamicBox.transform.rotation = Quaternion.Euler(0, 0, 0); //각도 고정 
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation; //플레이어가 들고있는 동안에는 모서리에 부딪혀도 회전하지 않음 

        dynamicBox.GetComponent<BoxCollider2D>().isTrigger = true; //잠시 플레이어와 반응하는 콜라이더 충돌 끄고 플랫폼과 반응하는 콜라이더만 남김 
        playerBoxColl.SetActive(true);
        
        while (isBoxGrab) //플레이어가 box를 잡고 있는 동안 
        {
            if (!playerObj.GetComponent<SpriteRenderer>().flipX)
            {
                dynamicBox.transform.position = playerObj.transform.position + initDistance + playerObj.transform.up * 0.5f; //box위치를 플레이어에 대해 일정하도록 고정 
                playerBoxColl.transform.position = dynamicBox.transform.position;
                yield return null;
            }
            else //플레이어가 flipX로 몸을 돌리면 상자는 반대쪽으로 움직여야 함 
            {
                dynamicBox.transform.position = playerObj.transform.position - initDistance + playerObj.transform.up * 0.5f; //box위치를 플레이어에 대해 일정하도록 고정 
                playerBoxColl.transform.position = dynamicBox.transform.position;
                yield return null;
            }            
        }

        //box 내려놓기        
        if (!playerObj.GetComponent<SpriteRenderer>().flipX)
        {
            dynamicBox.transform.position = dynamicBox.transform.position + 2*initDistance;
        }
        else
        {
            dynamicBox.transform.position = dynamicBox.transform.position - 2*initDistance;
        }

        rigid.constraints = RigidbodyConstraints2D.None;
        
        playerBoxColl.SetActive(false);
        dynamicBox.GetComponent<BoxCollider2D>().isTrigger = false; //다시 콜라이더 킴 
    }


    
    RaycastHit2D boxRay_top, boxRay_bottom, boxRay_right, boxRay_left;
    public int rayState;
    int flipOffset; // 1 or -1 

    void leverRay()
    {       
        boxRay_top = Physics2D.BoxCast(transform.position, new Vector2(0.2f, 0.4f), 0, new Vector2(0,1), 1.4f, LayerMask.GetMask("Player"));
        boxRay_bottom = Physics2D.BoxCast(transform.position, new Vector2(0.2f, 0.4f), 0, new Vector2(0, -1), 1.4f, LayerMask.GetMask("Player"));
        boxRay_right = Physics2D.BoxCast(transform.position, new Vector2(0.2f, 1), 0, new Vector2(1, 0), 1.4f, LayerMask.GetMask("Player"));
        boxRay_left = Physics2D.BoxCast(transform.position, new Vector2(0.2f, 1), 0, new Vector2(-1, 0), 1.4f, LayerMask.GetMask("Player"));

        // [0]: 플레이어 반응 없음 
        // [1] ~[4]: top, right, bottom, left 순서로 boxCast 반응 있음 

        if (boxRay_top.collider != null) rayState = 1;
        else if (boxRay_right.collider != null) rayState = 2;
        else if (boxRay_bottom.collider != null) rayState = 3;
        else if (boxRay_left.collider != null) rayState = 4;
        else rayState = 0;

        bool isPlayerFlip = playerObj.GetComponent<SpriteRenderer>().flipX;
        Vector3 playerHead = playerObj.transform.up; //플레이어의 머리방향 

        switch (rayState)
        {            
            case 1: //top box 반응    
                if( (playerHead == new Vector3(1, 0, 0) && !isPlayerFlip) || (playerHead == new Vector3(-1, 0, 0) && isPlayerFlip) )
                {
                    if (playerHead == Vector3.right) flipOffset = 1;
                    else flipOffset = -1;

                    isPlayerOn = true;
                }
                else
                {
                    isPlayerOn = false;
                }
                break;

            case 2: //right box 반응 
                if ((playerHead == new Vector3(0, 1, 0) && isPlayerFlip)|| (playerHead == new Vector3(0, -1, 0) && !isPlayerFlip))
                {
                    if (playerHead == Vector3.down) flipOffset = 1;
                    else flipOffset = -1;

                    isPlayerOn = true;
                }
                else
                {
                    isPlayerOn = false;
                }
                break;
                    
            case 3: //bottom box 반응 
                if ((playerHead == new Vector3(1, 0, 0) && isPlayerFlip) || (playerHead == new Vector3(-1, 0, 0) && !isPlayerFlip) )
                {
                    if (playerHead == Vector3.left) flipOffset = 1;
                    else flipOffset = -1;

                    isPlayerOn = true;
                }
                else
                {
                    isPlayerOn = false;
                }
                break;  

            case 4: //left box 반응 
                if ((playerHead == new Vector3(0, 1, 0) && !isPlayerFlip) || (playerHead == new Vector3(0, -1, 0) && isPlayerFlip))
                {
                    if (playerHead == Vector3.up) flipOffset = 1;
                    else flipOffset = -1;

                    isPlayerOn = true;
                }
                else
                {
                    isPlayerOn = false;
                }
                break;

            default:
                isPlayerOn = false;
                break;
        }       
    }
    
}
