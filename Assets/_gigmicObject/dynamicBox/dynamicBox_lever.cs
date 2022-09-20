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

    public float grabThreshold; //플레이어가 box를 잡을 수 있는 거리 기준 

    public float grabSpeed;
    float initWalkSpeed;
    float boxOffset;

    public int activeSavePointNum;
    //box가 작동하는 세이브포인트 번호
    //아직 해당 퍼즐에 진입하기 전에 박스가 자기 마음대로 움직이면 곤란함 

    int pastSavePoint;
    int curSavePoint;
    //매 프레임마다 현재의 진행도 체크 ~> 진행도가 달라질 때 마다 rigidBody 상태 그에 맞게 초기화해 줌 

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
        rigid.bodyType = RigidbodyType2D.Static;
        initWalkSpeed = playerscr.walkSpeed;
        isPlayerOn = false;
        isBoxGrab = false;

        curSavePoint = GameManager.instance.gameData.curAchievementNum;
        pastSavePoint = curSavePoint;

        if(curSavePoint == activeSavePointNum)
        {
            dynamicBox.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; //중력 영향 받아야 함 
        }
    }

    void Update()
    {
        curSavePoint = GameManager.instance.gameData.curAchievementNum;

        if(curSavePoint != pastSavePoint) //새로운 세이브포인트가 활성화되면
        {
            if(curSavePoint != activeSavePointNum)
            {
                dynamicBox.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static; //중력 영향 안받게 함 
                pastSavePoint = curSavePoint;
                return; //여타 동작 전부 stop 
            }
            else
            {
                dynamicBox.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic; //중력 영향 받아야 함 
                pastSavePoint = curSavePoint;
            }
        }
        else
        {
            pastSavePoint = curSavePoint;
        }

        leverRay();

        if(Input.GetKeyDown(KeyCode.E))
        {
            if (isPlayerOn && !isBoxGrab) //박스를 잡지 않은 상태에서 E를 누르면 박스 잡기
            {
                isBoxGrab = true;
                playerscr.isPlayerGrab = true;
                playerscr.walkSpeed = grabSpeed; //box들고있는 상태에선 더 느리게 걸음 

                StartCoroutine(boxGrab());
            }           

            else if (isBoxGrab)
            {
                isBoxGrab = false; //박스 잡고있는 상태에서 E 누르면 내려놓기 
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
                dynamicBox.GetComponent<BoxCollider2D>().isTrigger = false; //�ٽ� �ݶ��̴� Ŵ 
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("platform detected");
        if(collision.tag == "Platform")
        {

        }
    }

    Vector3 initDistance;
    
    IEnumerator boxGrab()
    {
        initDistance = dynamicBox.transform.position - playerObj.transform.position;
        if(playerObj.transform.up == new Vector3(0,1,0) || playerObj.transform.up == new Vector3(0, -1, 0)) //�÷��̾ 0, 180�� ������ �Ӹ��� �� �� 
        {
            initDistance = new Vector3(initDistance.x, 0, 0);
        }
        else //�÷��̾ 90, 270�� ������ �Ӹ��� �� �� 
        {
            initDistance = new Vector3(0, initDistance.y, 0);
        }

        initDistance = initDistance.normalized * 0.5f * flipOffset;
       
        dynamicBox.transform.rotation = Quaternion.Euler(0, 0, 0); //���� ���� 
        rigid.constraints = RigidbodyConstraints2D.FreezeRotation; //�÷��̾ ����ִ� ���ȿ��� �𼭸��� �ε���� ȸ������ ���� 

        dynamicBox.GetComponent<BoxCollider2D>().isTrigger = true; //��� �÷��̾�� �����ϴ� �ݶ��̴� �浹 ��� �÷����� �����ϴ� �ݶ��̴��� ���� 
        playerBoxColl.SetActive(true);
        
        while (isBoxGrab) //�÷��̾ box�� ��� �ִ� ���� 
        {
            if (!playerObj.GetComponent<SpriteRenderer>().flipX)
            {
                dynamicBox.transform.position = playerObj.transform.position + initDistance + playerObj.transform.up * 0.5f; //box��ġ�� �÷��̾ ���� �����ϵ��� ���� 
                playerBoxColl.transform.position = dynamicBox.transform.position;
                yield return null;
            }
            else //�÷��̾ flipX�� ���� ������ ���ڴ� �ݴ������� �������� �� 
            {
                dynamicBox.transform.position = playerObj.transform.position - initDistance + playerObj.transform.up * 0.5f; //box��ġ�� �÷��̾ ���� �����ϵ��� ���� 
                playerBoxColl.transform.position = dynamicBox.transform.position;
                yield return null;
            }            
        }

        //box ��������        
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
        dynamicBox.GetComponent<BoxCollider2D>().isTrigger = false; //�ٽ� �ݶ��̴� Ŵ 
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

        // [0]: �÷��̾� ���� ���� 
        // [1] ~[4]: top, right, bottom, left ������ boxCast ���� ���� 

        if (boxRay_top.collider != null) rayState = 1;
        else if (boxRay_right.collider != null) rayState = 2;
        else if (boxRay_bottom.collider != null) rayState = 3;
        else if (boxRay_left.collider != null) rayState = 4;
        else rayState = 0;

        bool isPlayerFlip = playerObj.GetComponent<SpriteRenderer>().flipX;
        Vector3 playerHead = playerObj.transform.up; //�÷��̾��� �Ӹ����� 

        switch (rayState)
        {            
            case 1: //top box ����    
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

            case 2: //right box ���� 
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
                    
            case 3: //bottom box ���� 
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

            case 4: //left box ���� 
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
