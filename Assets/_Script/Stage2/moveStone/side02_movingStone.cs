using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class side02_movingStone : MonoBehaviour
{
    public float moveSpeed;
    public int moveDir; //타입이 1이면 가로 이동, 2이면 세로 이동 
    public float targetPos; //현재 stone 이 이동하고 있는 목표 위치의 x or y 값 

    //type이 1이면 x좌표가 targetPos와 같아질 때 까지 이동한다 
    //type이 2이면 y좌표가 targetPos와 같아질 때 까지 이동한다 
    Rigidbody2D rigid;

    [SerializeField] bool shouldStop; //레버 조작이 없으면 stone은 멈추어야 함
    SpriteRenderer spr;
    public Sprite[] spriteGroup;
    /*
    0: 불 꺼짐
    1: 위쪽 화살표 
    2: 오른쪽 화살표
    3: 아래쪽 화살표
    4: 왼쪽 화살표
    */

    public bool isRightCollOn;
    public bool isLeftCollOn;
    public bool isTopCollOn;
    public bool isBottomCollOn;
    //stone이 움직이다가 벽에 부딪히면 멈춰야 함 ~> 충돌체크에 사용 

    public bool isOnMove; //움직이고 있는 동안은 레버 조작 무시 
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        spr.sprite = spriteGroup[0]; //기본sprite는 불이 안 들어온 버전 
    }

    // Update is called once per frame
    void Update()
    {
        stopCheck();
    }

    public void stoneMove(int moveType, float target) //타입이 1이면 가로 이동, 2이면 세로 이동 
    {
        Vector2 curPos = transform.position;
        targetPos = target; //목표위치 변경 
        moveDir = moveType;

        isOnMove = true;

        switch (moveType)
        {
            case 1:
                //rigid.constraints = RigidbodyConstraints2D.FreezePositionY; //x축 방향으로 이동하고 있을 땐 y축으로는 못움직이게 고정
                if(target > curPos.x)
                {
                    rigid.velocity = new Vector3(moveSpeed, 0, 0);
                    spr.sprite = spriteGroup[2]; //오른쪽 화살표 
                }
                else if(target < curPos.x)
                {
                    rigid.velocity = new Vector3(-moveSpeed, 0, 0);
                    spr.sprite = spriteGroup[4]; //왼쪽 화살표 
                }
                break;
           
            case 2:
                //rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
                if (target > curPos.y)
                {
                    rigid.velocity = new Vector3(0, moveSpeed, 0);
                    spr.sprite = spriteGroup[1]; //위쪽 화살표 
                }
                else if (target < curPos.y)
                {
                    rigid.velocity = new Vector3(0, -moveSpeed, 0);
                    spr.sprite = spriteGroup[3]; //아래쪽 화살표
                }
                break;
        }
    }

    void stopCheck()
    {
        if(moveDir == 1) //가로이동할 때 
        {
            if (Mathf.Abs(transform.position.x - targetPos) <= 0.05)
            {
                rigid.velocity = Vector3.zero; //목표위치에 근접하면 정지
                isOnMove = false;
                transform.position = new Vector3(targetPos, transform.position.y, 0);

                spr.sprite = spriteGroup[0];
            }

            if(rigid.velocity.x > 0 && isRightCollOn)
            {
                rigid.velocity = Vector3.zero; //목표위치에 근접하면 정지
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
            else if(rigid.velocity.x < 0 && isLeftCollOn)
            {
                rigid.velocity = Vector3.zero; //목표위치에 근접하면 정지
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
        }

        else if(moveDir == 2) //세로이동할 때
        {
            if (Mathf.Abs(transform.position.y - targetPos) <= 0.05)
            {
                rigid.velocity = Vector3.zero; //목표위치에 근접하면 정지
                isOnMove = false;
                transform.position = new Vector3(transform.position.x, targetPos, 0);

                spr.sprite = spriteGroup[0];
            }

            if (rigid.velocity.y > 0 && isTopCollOn)
            {
                rigid.velocity = Vector3.zero; //목표위치에 근접하면 정지
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
            else if (rigid.velocity.y < 0 && isBottomCollOn)
            {
                rigid.velocity = Vector3.zero; //목표위치에 근접하면 정지
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
        }

        
    }
}
