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

    public float grabThreshold; //�� �Ÿ� �̻� box�� �÷��̾ �־����� grab �� �����ȴ� 

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

        if(Input.GetKeyDown(KeyCode.E)) //E �� ������ �� 
        {
            if (isPlayerOn && !isBoxGrab) //�÷��̾ ���� ���� �ְ� ���� box�� ��� ���� ������ 
            {
                isBoxGrab = true;
                playerscr.isPlayerGrab = true;
                playerscr.walkSpeed = grabSpeed; //box ��������� �ӵ� ������ 

                StartCoroutine(boxGrab());
            }           

            else if (isBoxGrab)
            {
                isBoxGrab = false; //���� �������� 
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
