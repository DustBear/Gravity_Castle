using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slowFallingPlatform : MonoBehaviour
{
    public float fallSpeed; //������ �ӵ��� ��� ������ 
    public GameObject player; //�÷��̾��� ȸ������ �������� ���Ϲ����� ���� 

    public Vector2 pos1;
    public Vector2 pos2; //�̵� ���ɹ��� 
    /*
    ----pos2 
    --------
    pos1----     ������ pos2�� x,y �� �ϳ��� pos1 ���� Ŀ�� �� 
    */
    


    bool isMoveHorizontal; //�÷����� ���η� �����̸� true, ���η� �����̸� false
    int moveDirection;
    /*
    _ 1 _
    4 _ 2 //�÷��̾� �Ӹ��� ���ϴ� ���� 
    _ 3 _   
    */

    Rigidbody2D rigid;
    void Start()
    {
        if(pos1.x == pos2.x) //�� ������ x��ǥ�� ���� ~> y�� �������θ� �����̴� �÷����̴� 
        {
            isMoveHorizontal = false;
        }
        else
        {
            isMoveHorizontal = true;
        }

        rigid = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        directionCheck();
        speedCheck();
    }

    void directionCheck()
    {
        if (player.transform.up == new Vector3(0, 1, 0)) //�÷��̾� �Ӹ��� ������ ���� 
        {
            moveDirection = 1;
        }
        else if(player.transform.up == new Vector3(1, 0, 0)) //�÷��̾� �Ӹ��� �������� ���� 
        {
            moveDirection = 2;
        }
        else if (player.transform.up == new Vector3(0, -1, 0)) //�÷��̾� �Ӹ��� �Ʒ����� ���� 
        {
            moveDirection = 3;
        }
        else //�÷��̾� �Ӹ��� ������ ���� 
        {
            moveDirection = 4;
        }
    }

    void speedCheck()
    {
        if (isMoveHorizontal) //���η� �����̴� �÷����� ��: moveDir�� 1,3�� �� ������ �ְ� 2�϶� -x ��������, 4�� �� +x �������� �̵��ؾ� �� 
        {
            if(moveDirection==1 || moveDirection == 3)
            {
                rigid.velocity = Vector3.zero;
            }
            else if (moveDirection == 2)
            {
                if(transform.position.x <= pos1.x)
                {
                    transform.position = pos1;
                    rigid.velocity = Vector3.zero;
                }
                rigid.velocity = new Vector3(-fallSpeed, 0, 0);
            }
            else
            {
                if (transform.position.x >= pos2.x)
                {
                    transform.position = pos2;
                    rigid.velocity = Vector3.zero;
                }
                rigid.velocity = new Vector3(fallSpeed, 0, 0);
            }
        }
        else //���η� �����̴� �÷����� ��: moveDir�� 2,4�� �� ������ �ְ� 1�϶� -y ��������, 3�� �� +y �������� �̵��ؾ� �� 
        {
            if (moveDirection == 2 || moveDirection == 4)
            {
                rigid.velocity = Vector3.zero;
            }
            else if (moveDirection == 1)
            {
                if (transform.position.y <= pos1.y)
                {
                    transform.position = pos1;
                    rigid.velocity = Vector3.zero;
                }
                rigid.velocity = new Vector3(0, -fallSpeed, 0);
            }
            else
            {
                if (transform.position.y >= pos2.y)
                {
                    transform.position = pos2;
                    rigid.velocity = Vector3.zero;
                }
                rigid.velocity = new Vector3(0, fallSpeed, 0);
            }
        }
    }  
}