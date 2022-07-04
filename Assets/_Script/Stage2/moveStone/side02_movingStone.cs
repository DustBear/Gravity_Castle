using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class side02_movingStone : MonoBehaviour
{
    public float moveSpeed;
    public int moveDir; //Ÿ���� 1�̸� ���� �̵�, 2�̸� ���� �̵� 
    public float targetPos; //���� stone �� �̵��ϰ� �ִ� ��ǥ ��ġ�� x or y �� 

    //type�� 1�̸� x��ǥ�� targetPos�� ������ �� ���� �̵��Ѵ� 
    //type�� 2�̸� y��ǥ�� targetPos�� ������ �� ���� �̵��Ѵ� 
    Rigidbody2D rigid;

    [SerializeField] bool shouldStop; //���� ������ ������ stone�� ���߾�� ��
    SpriteRenderer spr;
    public Sprite[] spriteGroup;
    /*
    0: �� ����
    1: ���� ȭ��ǥ 
    2: ������ ȭ��ǥ
    3: �Ʒ��� ȭ��ǥ
    4: ���� ȭ��ǥ
    */

    public bool isRightCollOn;
    public bool isLeftCollOn;
    public bool isTopCollOn;
    public bool isBottomCollOn;
    //stone�� �����̴ٰ� ���� �ε����� ����� �� ~> �浹üũ�� ��� 

    public bool isOnMove; //�����̰� �ִ� ������ ���� ���� ���� 
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();

        spr.sprite = spriteGroup[0]; //�⺻sprite�� ���� �� ���� ���� 
    }

    // Update is called once per frame
    void Update()
    {
        stopCheck();
    }

    public void stoneMove(int moveType, float target) //Ÿ���� 1�̸� ���� �̵�, 2�̸� ���� �̵� 
    {
        Vector2 curPos = transform.position;
        targetPos = target; //��ǥ��ġ ���� 
        moveDir = moveType;

        isOnMove = true;

        switch (moveType)
        {
            case 1:
                //rigid.constraints = RigidbodyConstraints2D.FreezePositionY; //x�� �������� �̵��ϰ� ���� �� y�����δ� �������̰� ����
                if(target > curPos.x)
                {
                    rigid.velocity = new Vector3(moveSpeed, 0, 0);
                    spr.sprite = spriteGroup[2]; //������ ȭ��ǥ 
                }
                else if(target < curPos.x)
                {
                    rigid.velocity = new Vector3(-moveSpeed, 0, 0);
                    spr.sprite = spriteGroup[4]; //���� ȭ��ǥ 
                }
                break;
           
            case 2:
                //rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
                if (target > curPos.y)
                {
                    rigid.velocity = new Vector3(0, moveSpeed, 0);
                    spr.sprite = spriteGroup[1]; //���� ȭ��ǥ 
                }
                else if (target < curPos.y)
                {
                    rigid.velocity = new Vector3(0, -moveSpeed, 0);
                    spr.sprite = spriteGroup[3]; //�Ʒ��� ȭ��ǥ
                }
                break;
        }
    }

    void stopCheck()
    {
        if(moveDir == 1) //�����̵��� �� 
        {
            if (Mathf.Abs(transform.position.x - targetPos) <= 0.05)
            {
                rigid.velocity = Vector3.zero; //��ǥ��ġ�� �����ϸ� ����
                isOnMove = false;
                transform.position = new Vector3(targetPos, transform.position.y, 0);

                spr.sprite = spriteGroup[0];
            }

            if(rigid.velocity.x > 0 && isRightCollOn)
            {
                rigid.velocity = Vector3.zero; //��ǥ��ġ�� �����ϸ� ����
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
            else if(rigid.velocity.x < 0 && isLeftCollOn)
            {
                rigid.velocity = Vector3.zero; //��ǥ��ġ�� �����ϸ� ����
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
        }

        else if(moveDir == 2) //�����̵��� ��
        {
            if (Mathf.Abs(transform.position.y - targetPos) <= 0.05)
            {
                rigid.velocity = Vector3.zero; //��ǥ��ġ�� �����ϸ� ����
                isOnMove = false;
                transform.position = new Vector3(transform.position.x, targetPos, 0);

                spr.sprite = spriteGroup[0];
            }

            if (rigid.velocity.y > 0 && isTopCollOn)
            {
                rigid.velocity = Vector3.zero; //��ǥ��ġ�� �����ϸ� ����
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
            else if (rigid.velocity.y < 0 && isBottomCollOn)
            {
                rigid.velocity = Vector3.zero; //��ǥ��ġ�� �����ϸ� ����
                isOnMove = false;
                spr.sprite = spriteGroup[0];
            }
        }

        
    }
}
